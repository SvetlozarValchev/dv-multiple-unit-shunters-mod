using System;
using System.Collections.Generic;
using System.Reflection;
using UnityModManagerNet;
using Harmony12;
using UnityEngine;

namespace MultipleUnitShuntersMod
{
    public class Main
    {
        static bool Load(UnityModManager.ModEntry modEntry)
        {
            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            // Something
            return true; // If false the mod will show an error.
        }
    }

    [HarmonyPatch(typeof(LocoControllerShunter), "SetThrottle")]
    class LocoControllerShunter_SetThrottle_Patch
    {
        static void Postfix(LocoControllerShunter __instance, float throttleLever)
        {
            TrainCar currentCar = __instance.GetComponent<TrainCar>();

            if (currentCar == null || !PlayerManager.Car || !PlayerManager.Car.Equals(currentCar))
            {
                return;
            }

            Trainset trainset = PlayerManager.Trainset;

            if (trainset == null)
            {
                return;
            }

            for (int i = 0; i < trainset.cars.Count; i++)
            {
                TrainCar car = trainset.cars[i];

                if (PlayerManager.Car.Equals(car))
                {
                    continue;
                }

                if (trainset.cars[i].carType == TrainCarType.LocoShunter)
                {
                    LocoControllerShunter locoController = trainset.cars[i].GetComponent<LocoControllerShunter>();

                    if (locoController)
                    {
                        locoController.SetThrottle(throttleLever);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(LocoControllerShunter), "SetReverser")]
    class LocoControllerShunter_SetReverser_Patch
    {
        static void Postfix(LocoControllerShunter __instance, float position)
        {
            TrainCar currentCar = __instance.GetComponent<TrainCar>();

            if (currentCar == null || !PlayerManager.Car || !PlayerManager.Car.Equals(currentCar))
            {
                return;
            }

            Trainset trainset = PlayerManager.Trainset;

            if (trainset == null)
            {
                return;
            }

            for (int i = 0; i < trainset.cars.Count; i++)
            {
                TrainCar car = trainset.cars[i];

                if (PlayerManager.Car.Equals(car))
                {
                    continue;
                }

                if (trainset.cars[i].carType == TrainCarType.LocoShunter)
                {
                    LocoControllerShunter locoController = trainset.cars[i].GetComponent<LocoControllerShunter>();

                    if (locoController)
                    {
                        locoController.SetReverser(position);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(LocoControllerShunter), "SetBrake")]
    class LocoControllerShunter_SetBrake_Patch
    {
        static void Postfix(LocoControllerShunter __instance, float brake)
        {
            TrainCar currentCar = __instance.GetComponent<TrainCar>();

            if (currentCar == null || !PlayerManager.Car || !PlayerManager.Car.Equals(currentCar))
            {
                return;
            }

            Trainset trainset = PlayerManager.Trainset;

            if (trainset == null)
            {
                return;
            }

            for (int i = 0; i < trainset.cars.Count; i++)
            {
                TrainCar car = trainset.cars[i];

                if (PlayerManager.Car.Equals(car))
                {
                    continue;
                }

                if (trainset.cars[i].carType == TrainCarType.LocoShunter)
                {
                    LocoControllerShunter locoController = trainset.cars[i].GetComponent<LocoControllerShunter>();

                    if (locoController)
                    {
                        locoController.SetBrake(brake);
                    }
                }
            }
        }
    }
}
