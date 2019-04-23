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
        public static TrainCar remoteCar;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            return true;
        }
    }

    // throttle remote control
    [HarmonyPatch(typeof(LocoControllerBase), "UpdateThrottle")]
    class LocoControllerBase_UpdateThrottle_Patch
    {
        static void Prefix(LocoControllerBase __instance)
        {
            Main.remoteCar = __instance.GetComponent<TrainCar>();
        }
    }

    // throttle
    [HarmonyPatch(typeof(LocoControllerShunter), "SetThrottle")]
    class LocoControllerShunter_SetThrottle_Patch
    {
        static void Postfix(LocoControllerShunter __instance, float throttleLever)
        {
            TrainCar currentCar = __instance.GetComponent<TrainCar>();
            TrainCar targetCar;
            Trainset trainset;

            if (Main.remoteCar)
            {
                targetCar = Main.remoteCar;
                trainset = Trainset.GetFromCar(targetCar);
            }
            else
            {
                targetCar = PlayerManager.Car;
                trainset = PlayerManager.Trainset;
            }

            if (currentCar == null || !targetCar || !targetCar.Equals(currentCar) || trainset == null || trainset.cars.Count < 2)
            {
                return;
            }

            for (int i = 0; i < trainset.cars.Count; i++)
            {
                TrainCar car = trainset.cars[i];

                if (targetCar.Equals(car))
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

            Main.remoteCar = null;
        }
    }

    // reverser remote control
    [HarmonyPatch(typeof(LocoControllerBase), "UpdateReverser")]
    class LocoControllerBase_UpdateReverser_Patch
    {
        static void Prefix(LocoControllerBase __instance)
        {
            Main.remoteCar = __instance.GetComponent<TrainCar>();
        }
    }

    // reverser
        [HarmonyPatch(typeof(LocoControllerShunter), "SetReverser")]
    class LocoControllerShunter_SetReverser_Patch
    {
        static void Postfix(LocoControllerShunter __instance, float position)
        {
            TrainCar currentCar = __instance.GetComponent<TrainCar>();
            TrainCar targetCar;
            Trainset trainset;

            if (Main.remoteCar)
            {
                targetCar = Main.remoteCar;
                trainset = Trainset.GetFromCar(targetCar);
            } else
            {
                targetCar = PlayerManager.Car;
                trainset = PlayerManager.Trainset;
            }

            if (currentCar == null || !targetCar || !targetCar.Equals(currentCar) || trainset == null || trainset.cars.Count < 2)
            {
                return;
            }

            List<TrainCar> trainsetCars = trainset.cars;

            for (int i = 0; i < trainsetCars.Count; i++)
            {
                TrainCar car = trainsetCars[i];

                if (targetCar.Equals(car))
                {
                    continue;
                }

                if (car.carType == TrainCarType.LocoShunter)
                {
                    LocoControllerShunter locoController = car.GetComponent<LocoControllerShunter>();

                    if (locoController)
                    {
                        if (Trainset.GetCarsBehind(targetCar).Contains(car))
                        {
                            if (Trainset.GetCarsInFrontOf(car).Contains(targetCar))
                            {
                                locoController.SetReverser(position);
                            } else
                            {
                                locoController.SetReverser(position * -1f);
                            }
                        }
                        else if (Trainset.GetCarsInFrontOf(targetCar).Contains(car))
                        {
                            if (Trainset.GetCarsBehind(car).Contains(targetCar))
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

            Main.remoteCar = null;
        }
    }

    // brake remote control
    [HarmonyPatch(typeof(LocoControllerBase), "UpdateBrake")]
    class LocoControllerBase_UpdateBrake_Patch
    {
        static void Prefix(LocoControllerBase __instance)
        {
            Main.remoteCar = __instance.GetComponent<TrainCar>();
        }
    }

    // brake
    [HarmonyPatch(typeof(LocoControllerBase), "SetBrake")]
    class LocoControllerBase_SetBrake_Patch
    {
        static void Postfix(LocoControllerBase __instance, float brake)
        {
            TrainCar currentCar = __instance.GetComponent<TrainCar>();
            TrainCar targetCar;
            Trainset trainset;

            Debug.Log("part 1");

            if (Main.remoteCar)
            {
                targetCar = Main.remoteCar;
                trainset = Trainset.GetFromCar(targetCar);
            }
            else
            {
                targetCar = PlayerManager.Car;
                trainset = PlayerManager.Trainset;
            }

            Debug.Log("part 2");

            if (currentCar == null || !targetCar || !targetCar.Equals(currentCar) || trainset == null || trainset.cars.Count < 2)
            {
                return;
            }

            Debug.Log("part 3");

            for (int i = 0; i < trainset.cars.Count; i++)
            {
                TrainCar car = trainset.cars[i];

                if (targetCar.Equals(car))
                {
                    continue;
                }

                Debug.Log("part 4");

                if (car.carType == TrainCarType.LocoShunter)
                {
                    LocoControllerShunter locoController = car.GetComponent<LocoControllerShunter>();

                    Debug.Log("part 5");

                    if (locoController)
                    {
                        Debug.Log("part 6");

                        locoController.SetBrake(brake);
                    }
                }
            }

            Main.remoteCar = null;
        }
    }

    // sand remote control
    [HarmonyPatch(typeof(LocoControllerShunter), "UpdateSand")]
    class LocoControllerShunter_UpdateSand_Patch
    {
        static void Prefix(LocoControllerShunter __instance, ToggleDirection toggle)
        {
            TrainCar targetCar = __instance.GetComponent<TrainCar>();
            Trainset trainset = Trainset.GetFromCar(targetCar);

            if (trainset == null)
            {
                return;
            }

            for (int i = 0; i < trainset.cars.Count; i++)
            {
                TrainCar car = trainset.cars[i];

                if (targetCar.Equals(car))
                {
                    continue;
                }

                if (car.carType == TrainCarType.LocoShunter)
                {
                    LocoControllerShunter locoController = car.GetComponent<LocoControllerShunter>();

                    if (locoController)
                    {
                        locoController.SetSandersOn(toggle == ToggleDirection.UP);
                    }
                }
            }
        }
    }

    // sander & fan
    [HarmonyPatch(typeof(ShunterDashboardControls), "OnEnable")]
    class ShunterDashboardControls_OnEnable_Patch2
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
                            locoController.SetSandersOn(e.newValue >= 0.5f);
                        }
                    }
                }
            });

            DV.CabControls.ControlImplBase fanCtrl = instance.fanSwitchButton.GetComponent<DV.CabControls.ControlImplBase>();

            fanCtrl.ValueChanged += (e =>
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
                            locoController.SetFan(e.newValue >= 0.5f);
                        }
                    }
                }
            });
        }
    }
}
