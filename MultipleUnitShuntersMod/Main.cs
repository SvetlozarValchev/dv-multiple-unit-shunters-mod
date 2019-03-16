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

                if (car.carType == TrainCarType.LocoShunter)
                {
                    LocoControllerShunter locoController = car.GetComponent<LocoControllerShunter>();

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

            if (trainset == null || trainset.cars.Count < 2)
            {
                return;
            }

            List<TrainCar> trainsetCars = trainset.cars;

            for (int i = 0; i < trainsetCars.Count; i++)
            {
                TrainCar car = trainsetCars[i];

                if (PlayerManager.Car.Equals(car))
                {
                    continue;
                }

                if (car.carType == TrainCarType.LocoShunter)
                {
                    LocoControllerShunter locoController = car.GetComponent<LocoControllerShunter>();

                    if (locoController)
                    {
                        if (Trainset.GetCarsBehind(PlayerManager.Car).Contains(car))
                        {
                            if (Trainset.GetCarsInFrontOf(car).Contains(PlayerManager.Car))
                            {
                                locoController.SetReverser(position);
                            } else
                            {
                                locoController.SetReverser(position * -1f);
                            }
                        }
                        else if (Trainset.GetCarsInFrontOf(PlayerManager.Car).Contains(car))
                        {
                            if (Trainset.GetCarsBehind(car).Contains(PlayerManager.Car))
                            {
                                locoController.SetReverser(position);
                            }
                            else
                            {
                                locoController.SetReverser(position * -1f);
                            }
                        }
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

                if (car.carType == TrainCarType.LocoShunter)
                {
                    LocoControllerShunter locoController = car.GetComponent<LocoControllerShunter>();

                    if (locoController)
                    {
                        locoController.SetBrake(brake);
                    }
                }
            }
        }
    }
    
    [HarmonyPatch(typeof(ShunterDashboardControls), "OnEnable")]
    class ShunterDashboardControls_OnEnable_Patch
    {
        static ShunterDashboardControls instance;

        static void Postfix(ShunterDashboardControls __instance)
        {
            instance = __instance;

            __instance.StartCoroutine(AttachListeners());
        }

        static IEnumerator<object> AttachListeners()
        {
            yield return (object)null;

            DV.CabControls.ControlImplBase sandDeployCtrl = instance.sandDeployBtn.GetComponent<DV.CabControls.ControlImplBase>();
            
            sandDeployCtrl.ValueChanged += (e =>
            {
                if (PlayerManager.Trainset == null) return;

                for (int i = 0; i < PlayerManager.Trainset.cars.Count; i++)
                {
                    TrainCar car = PlayerManager.Trainset.cars[i];

                    if (PlayerManager.Car.Equals(car))
                    {
                        continue;
                    }

                    if (car.carType == TrainCarType.LocoShunter)
                    {
                        LocoControllerShunter locoController = car.GetComponent<LocoControllerShunter>();

                        if (locoController)
                        {
                            locoController.SetSandersOn(Convert.ToBoolean(e.newValue));
                        }
                    }
                }
            });
        }
    }
}
