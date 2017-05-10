using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Shared.Math;

namespace spl_gasstations.Server
{
    class main : Script
    {
        public main()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            var fuel_price = 1.39f * 2;
            switch (eventName)
            {
                case "refuel_full":
                    sender.vehicle.engineStatus = false;
                    sender.vehicle.freezePosition = true;
                    dynamic fuel_level = sender.vehicle.getSyncedData("fuel_level");
                    var cost = 0.0;
                    while (fuel_level <= 100)
                    {
                        fuel_level += fuel_price;
                        cost += fuel_price;
                        sender.vehicle.setSyncedData("fuel_level", fuel_level);
                        API.sleep(1000);
                    }
                    sender.vehicle.freezePosition = false;
                    sender.vehicle.engineStatus = true;
                    cost = Math.Ceiling(cost);
                    API.exported.spl_economy_money.BankTransaction(sender, Convert.ToInt32(-cost));
                    sender.sendChatMessage("~b~Refueling done! Paid ~g~$ "+ cost.ToString());

                    break;
                case "refuel_amount":
                    var fuel_level_origin = 0.0;
                    sender.vehicle.engineStatus = false;
                    sender.vehicle.freezePosition = true;
                    double fuel_level2 = fuel_level_origin = Convert.ToDouble(sender.vehicle.getSyncedData("fuel_level"));
                    cost = 0.0;
                    while (fuel_level2 <= Clamp(fuel_level_origin + Convert.ToDouble(arguments[0]), 0f, 100f))
                    {
                        fuel_level2 += fuel_price;
                        cost += fuel_price;
                        sender.vehicle.setSyncedData("fuel_level", fuel_level2);
                        API.sleep(1000);
                    }
                    sender.vehicle.freezePosition = false;
                    sender.vehicle.engineStatus = true;
                    cost = Math.Ceiling(cost);
                    API.exported.spl_economy_money.BankTransaction(sender, Convert.ToInt32(-cost));
                    sender.sendChatMessage("~b~Refueling done! Paid ~g~$ " + cost.ToString());
                    break;
            }
        }
        [Command]
        public void unstuck(Client p)
        {
            p.vehicle.freezePosition = false;
        }
        private double Clamp(double val, double min, double max)
        {
            return Math.Max(min, Math.Min(val, max));
        }

        public void onCollisionEnter(Client p, ColShape c)
        {
            if(p.vehicleSeat == -1)
            {
                API.sendNativeToPlayer(p, Hash.SET_VEHICLE_FORWARD_SPEED, p.vehicle, 0.0f);
                API.triggerClientEvent(p, "show_menu");
            }
        }
        public void onCollisionExit(Client p, ColShape c)
        {
            if (p.vehicleSeat == -1)
            {
                API.triggerClientEvent(p, "hide_menu");
            }
        }
        dynamic Stations;
        private void API_onResourceStart()
        {
            using (StreamReader r = new StreamReader("resources/spl_gasstations/stations.json"))
            {
                string json = r.ReadToEnd();
                Stations = JsonConvert.DeserializeObject(json);
            }
            foreach (var item in Stations)
            {
                var m =API.createMarker(
                    1,
                    new Vector3(
                        Convert.ToInt32(item.Position.X),
                        Convert.ToInt32(item.Position.Y),
                        Convert.ToInt32(item.Position.Z)
                    ),
                    new Vector3(),
                    new Vector3(),
                    new Vector3(1, 1, 1),
                    128,
                    0,
                    255,
                    0
                );
                var c = API.createCylinderColShape(new Vector3(Convert.ToInt32(item.Position.X), Convert.ToInt32(item.Position.Y), Convert.ToInt32(item.Position.Z)), 1, 2.5f);
                c.setData("Type", "Refuel");
            }
        }
    }
}
