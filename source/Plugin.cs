using Newtonsoft.Json;
using Rocket.Core.Plugins;
using SDG.NetTransport;
using SDG.Unturned;
using Steamworks;
using System;
using System.IO;
using UnityEngine;

namespace BetterCarExperience
{
    public class Plugin : RocketPlugin
    {
        public Configuration Configuration { get; private set; }

        private static readonly ClientStaticMethod<uint, byte, CSteamID> SendEnterVehicle = ClientStaticMethod<uint, byte, CSteamID>.Get(VehicleManager.ReceiveEnterVehicle);

        protected override void Load()
        {
            base.Load();

            LoadConfiguration();

            VehicleManager.onEnterVehicleRequested += OnPlayerEnterVehicleRequested;
            VehicleManager.onExitVehicleRequested += OnPlayerExitVehicleRequested;
            VehicleManager.onSwapSeatRequested += OnPlayerSwapSeatRequested;
        }

        private void OnPlayerEnterVehicleRequested(Player player, InteractableVehicle vehicle, ref bool shouldAllow)
        {   
            int nearestSeatIndex = GetNearestSeatIndex(player, vehicle);
            shouldAllow = false;

            if (nearestSeatIndex == -1)
            {
                return;
            }

            byte seatIndex = (byte)nearestSeatIndex;

            if (vehicle.passengers[seatIndex].player != null)
            {
                return;
            }

            CSteamID playerID = player.channel.owner.playerID.steamID;
            uint vehicleID = vehicle.instanceID;

            SendPlayerEnterVehicle(playerID, vehicleID, seatIndex);
        }

        private void OnPlayerExitVehicleRequested(Player player, InteractableVehicle vehicle, ref bool shouldAllow, ref Vector3 pendingLocation, ref float pendingYaw)
        {
            shouldAllow = IsSpeedAllowed(vehicle.ReplicatedSpeed * 3.6f);
        }

        private void OnPlayerSwapSeatRequested(Player player, InteractableVehicle vehicle, ref bool shouldAllow, byte fromSeatIndex, ref byte toSeatIndex)
        {
            shouldAllow = IsSpeedAllowed(vehicle.ReplicatedSpeed * 3.6f);
        }

        private bool IsSpeedAllowed(float speed)
        {
            return speed < Configuration.MaxSpeed;
        }

        private int GetNearestSeatIndex(Player player, InteractableVehicle vehicle)
        {
            float nearestSeatDistance = float.MaxValue;
            int nearestSeatIndex = -1;

            for (int i = 0; i < vehicle.passengers.Length; i++)
            {
                if (vehicle.passengers[i].player != null)
                    continue;

                Vector3 seatPosition = vehicle.passengers[i].seat.transform.position;

                float distance = Vector3.Distance(player.transform.position, seatPosition);

                if (distance < nearestSeatDistance)
                {
                    nearestSeatDistance = distance;
                    nearestSeatIndex = i;
                }
            }

            return nearestSeatIndex;
        }
        
        private void SendPlayerEnterVehicle(CSteamID playerID, uint vehicleID, byte seatIndex)
        {
            SendEnterVehicle.InvokeAndLoopback(ENetReliability.Reliable, Provider.GatherRemoteClientConnections(), vehicleID, seatIndex, playerID);
        }

        protected override void Unload()
        {
            base.Unload();

            VehicleManager.onEnterVehicleRequested -= OnPlayerEnterVehicleRequested;
            VehicleManager.onExitVehicleRequested -= OnPlayerExitVehicleRequested;
            VehicleManager.onSwapSeatRequested -= OnPlayerSwapSeatRequested;
        }

        private void LoadConfiguration()
        {
            string configPath = Path.Combine(Directory, "BetterCarExperience.json");

            if (!File.Exists(configPath))
            {
                Configuration = new Configuration{ MaxSpeed = 15f };
                SaveConfiguration();
            }

            else
            {
                string json = File.ReadAllText(configPath);
                Configuration = JsonConvert.DeserializeObject<Configuration>(json);
            }
        }

        private void SaveConfiguration()
        {
            string configPath = Path.Combine(Directory, "BetterCarExperience.json");

            string json = JsonConvert.SerializeObject(Configuration, Formatting.Indented);

            File.WriteAllText(configPath, json);
        }
    }
}
