# GyroHelpers
A small C# library to make adding Gyro Aiming to games easier. Features basic gyro calibration, Player Space gyro, Flick Stick, and more.  
Based on articles from [GyroWiki](http://gyrowiki.jibbsmart.com/) and [GamepadMotionHelpers](https://github.com/JibbSmart/GamepadMotionHelpers/).  

Requires another library for reading controller input, i.e. [SDL3-CS](https://www.nuget.org/packages/ppy.SDL3-CS).  

## Units and Coordinate System
GyroHelpers is designed with SDL3 in mind. If using a different library, inputs should be converted to [it's equivalents](https://wiki.libsdl.org/SDL3/SDL_SensorType#remarks).  
* **Gyro:** Radians per second. `X = Pitch, Y = Yaw, Z = Roll`  
* **Accelerometer:** Meters per second squared. `X = Left/Right, Y = Down/Up, Z = Farther/Closer`  
* **Timestamps:** Nanoseconds.  

## Gyro Calibration
The first step is to calibrate gyro and calculate gravity, which is done with the `GyroInput` class.  

At the start of the frame, before polling input, call `GyroInput.Begin()`.  
For every received gyro sample, call `GyroInput.AddGyroSample(Vector3 gyro, ulong timestamp)`.  
For every received accelerometer sample, call `GyroInput.AddAccelerometerSample(Vector3 accelerometer, ulong timestamp)`.  
After all samples are received, you can query the result with `GyroInput.Gyro`.  

To calibrate the gyro and correct for drift, set `GyroInput.Calibrating`.  

## Gyro Processing
For most cases, you can process the gyro data with `GyroProcessor`.  
Feed `GyroInput.Gyro` into `GyroProcessor.Update(Gyroscope gyro, float deltaTime)`.  
The result can added to the camera's pitch and yaw. Multiply this by a sensitivity slider.  
If you need the result in degrees, multiply by `MathHelper.RadiansToDegrees`.  

If you want more control over the processing chain, consider implementing your own class.  
For most purposes, a `GyroSpace` and `GyroProcessor.ApplyTightening` is enough.  

## Flick Stick
For Flick Stick, use the `FlickStick` class.  
Feed the controller's right stick into `FlickStick.Update(Vector2 stick, float deltaTime)` every frame.  
The result can be added to the camera's the yaw axis.  
If you need the result in degrees, multiply by `MathHelper.RadiansToDegrees`.  

## Example with SDL3-CS
This won't work on it's own! Only use this as a starting point for your own implementation.  
Read the [SDL3 Documentation](https://wiki.libsdl.org/SDL3/CategoryGamepad) for more information.  

```csharp
using GyroHelpers;
using SDL;
using static SDL.SDL3;

// Create processing classes.
// Simplified for this example. Ideally these should be created per controller.
GyroInput gyroInput = new GyroInput();
GyroProcessor gyroProcessor = new GyroProcessor();

// A function that gets called every frame.
public unsafe void Update(float deltaTime)
{
    // Call GyroInput.Begin() before processing sensor data.
    gyroInput.Begin();

    // Poll inputs using SDL3.
    SDL_Event evnt;
    while (SDL_PollEvent(&evnt))
    {
        switch((SDL_EventType)evnt.type){
            case SDL_EventType.SDL_EVENT_GAMEPAD_ADDED:
                // Open gamepad and enable sensors.
                SDL_Gamepad* gamepad = SDL_OpenGamepad(evnt.gdevice.which);
                SDL_SetGamepadSensorEnabled(gamepad, SDL_SensorType.SDL_SENSOR_GYRO, true);
                SDL_SetGamepadSensorEnabled(gamepad, SDL_SensorType.SDL_SENSOR_ACCEL, true);
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_SENSOR_UPDATE:
                // Feed sensor data into GyroInput.
                ulong timestamp = evnt.gsensor.sensor_timestamp;
                Vector3 data = new Vector3(evnt.gsensor.data[0], evnt.gsensor.data[1], evnt.gsensor.data[2]);
                switch ((SDL_SensorType)evnt.gsensor.sensor)
                {
                    case SDL_SensorType.SDL_SENSOR_GYRO:
                        gyroInput.AddGyroSample(data, timestamp);
                        break;
                    case SDL_SensorType.SDL_SENSOR_ACCEL:
                        gyroInput.AddAccelerometerSample(data, timestamp);
                        break;
                }
                break;
        }
    }

    // Process the result. Use this to rotate the camera.
    Vector2 angleDelta = gyroProcessor.Update(gyroInput.Gyro, deltaTime);
}
```