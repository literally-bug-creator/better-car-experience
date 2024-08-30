# 🚗 BetterCarExperience Plugin

**BetterCarExperience** is a plugin for **Unturned** that overrides the default behavior of entering and exiting vehicles. The plugin intelligently places players into the closest available seat when entering a vehicle and restricts exiting or switching seats when the vehicle is moving at high speeds.

## 🛠️ Features

1. **Closest Seat Assignment**: Players will automatically sit in the seat closest to their position when entering a vehicle. No more random seat assignments—just the most logical and nearest spot!

2. **Exit Restriction Based on Speed**: Players cannot exit the vehicle if the vehicle's speed exceeds a configurable limit, preventing dangerous dismounts at high velocities.

3. **Seat Switching Restriction**: Players are restricted from switching seats when the vehicle's speed exceeds a configurable limit, ensuring that players cannot move freely between seats while the vehicle is moving too fast.

## ⚙️ Configuration

The plugin comes with a single configuration field that allows you to set the maximum speed at which players can switch seats or exit the vehicle.

```json
{
  "MaxSpeed": 15.0
}
```

MaxSpeed: The speed threshold, above which players are prevented from exiting or switching seats. This value is a float and can be adjusted to suit your server's needs. The default is 15.0.
