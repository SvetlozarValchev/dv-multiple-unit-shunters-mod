using System;
using System.Collections.Generic;
using System.Reflection;
using UnityModManagerNet;
using Harmony12;
using UnityEngine;

namespace MultipleUnitDieselsMod
{
    public class Main
    {
        public static TrainCar remoteCar;
        private static bool listenersSetup = false;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            modEntry.OnUpdate = OnUpdate;

            return true;
        }

        static void OnUpdate(UnityModManager.ModEntry mod, float delta)
        {
            if (!listenersSetup)
            {
                // Gotta wait until we are loaded until registering the listeners
                if (LoadingScreenManager.IsLoading || !WorldStreamingInit.IsLoaded || !InventoryStartingItems.itemsLoaded) return;

                Grabber grab = PlayerManager.PlayerTransform.GetComponentInChildren<Grabber>();
                grab.Grabbed += OnItemGrabbedRightNonVR;
                grab.Released += OnItemUngrabbedRightNonVR;
                SingletonBehaviour<Inventory>.Instance.ItemAddedToInventory += OnItemAddedToInventory;

                mod.Logger.Log("Listeners have been set up.");
                listenersSetup = true;
            }
        }

        // Need to know when we have grabbed a Locomotive Remote
        // Actual Grab Handlers
        static void OnItemGrabbedRight(InventoryItemSpec iis)
        {
            LocomotiveRemoteController lrc = iis?.GetComponent<LocomotiveRemoteController>();

            LocoControllerBase locoController = (LocoControllerBase)typeof(LocomotiveRemoteController).GetField("pairedLocomotive", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(lrc);
            remoteCar = locoController.GetComponent<TrainCar>();
        }
        static void OnItemUngrabbedRight() { remoteCar = null; }
        // Grab Listeners
        static void OnItemAddedToInventory(GameObject o, int _) { OnItemUngrabbedRight(); }
        static void OnItemGrabbedRightNonVR(GameObject o) { OnItemGrabbedRight(o.GetComponent<InventoryItemSpec>()); }
        static void OnItemUngrabbedRightNonVR(GameObject o) { OnItemUngrabbedRight(); }
    }

    // throttle
    [HarmonyPatch(typeof(LocoControllerShunter), "SetThrottle")]
    class LocoControllerShunter_SetThrottle_Patch
    {
        static void Postfix(LocoControllerShunter __instance, float throttleLever)
        {
            TrainCar currentCar = __instance.GetComponent<TrainCar>();
            TrainCar targetCar = null;
            Trainset trainset = null;

            if (Main.remoteCar)
            {
                targetCar = Main.remoteCar;
                trainset = targetCar.trainset;
            }
            else if (PlayerManager.Car != null)
            {
                targetCar = PlayerManager.Car;
                trainset = PlayerManager.Car.trainset;
            }

            if (currentCar == null || targetCar == null || !targetCar.Equals(currentCar) || trainset == null || trainset.cars.Count < 2)
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

                LocoControllerBase locoController = null;

                if (car.carType == TrainCarType.LocoDiesel)
                {
                    locoController = car.GetComponent<LocoControllerDiesel>();
                }
                else if (car.carType == TrainCarType.LocoShunter)
                {
                    locoController = car.GetComponent<LocoControllerShunter>();
                }

                if (locoController != null)
                {
                    locoController.SetThrottle(throttleLever);
                }
            }
        }
    }

    // throttle
    [HarmonyPatch(typeof(LocoControllerDiesel), "SetThrottle")]
    class LocoControllerDiesel_SetThrottle_Patch
    {
        static void Postfix(LocoControllerDiesel __instance, float throttleLever)
        {
            TrainCar currentCar = __instance.GetComponent<TrainCar>();
            TrainCar targetCar = null;
            Trainset trainset = null;

            if (Main.remoteCar)
            {
                targetCar = Main.remoteCar;
                trainset = targetCar.trainset;
            }
            else if (PlayerManager.Car != null)
            {
                targetCar = PlayerManager.Car;
                trainset = PlayerManager.Car.trainset;
            }

            if (currentCar == null || targetCar == null || !targetCar.Equals(currentCar) || trainset == null || trainset.cars.Count < 2)
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

                LocoControllerBase locoController = null;

                if (car.carType == TrainCarType.LocoDiesel)
                {
                    locoController = car.GetComponent<LocoControllerDiesel>();
                } else if (car.carType == TrainCarType.LocoShunter)
                {
                    locoController = car.GetComponent<LocoControllerShunter>();
                }

                if (locoController != null)
                {
                    locoController.SetThrottle(throttleLever);
                }
            }
        }
    }

    // reverser
    [HarmonyPatch(typeof(LocoControllerShunter), "SetReverser")]
    class LocoControllerShunter_SetReverser_Patch
    {
        static void Postfix(LocoControllerShunter __instance, float position)
        {
            TrainCar currentCar = __instance.GetComponent<TrainCar>();
            TrainCar targetCar = null;
            Trainset trainset = null;

            if (Main.remoteCar)
            {
                targetCar = Main.remoteCar;
                trainset = targetCar.trainset;
            }
            else if (PlayerManager.Car != null)
            {
                targetCar = PlayerManager.Car;
                trainset = PlayerManager.Car.trainset;
            }

            if (currentCar == null || targetCar == null || !targetCar.Equals(currentCar) || trainset == null || trainset.cars.Count < 2)
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

                LocoControllerBase locoController = null;

                if (car.carType == TrainCarType.LocoShunter)
                {
                     locoController = car.GetComponent<LocoControllerShunter>();
                } else if (car.carType == TrainCarType.LocoDiesel)
                {
                    locoController = car.GetComponent<LocoControllerDiesel>();
                }

                if (locoController != null)
                {
                    if (GetCarsBehind(targetCar).Contains(car))
                    {
                        if (GetCarsInFrontOf(car).Contains(targetCar))
                        {
                            locoController.SetReverser(position);
                        }
                        else
                        {
                            locoController.SetReverser(position * -1f);
                        }
                    }
                    else if (GetCarsInFrontOf(targetCar).Contains(car))
                    {
                        if (GetCarsBehind(car).Contains(targetCar))
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

        public static List<TrainCar> GetCarsInFrontOf(TrainCar car)
        {
            return GetCarsCoupledTo(car.frontCoupler);
        }

        public static List<TrainCar> GetCarsBehind(TrainCar car)
        {
            return GetCarsCoupledTo(car.rearCoupler);
        }

        public static List<TrainCar> GetCarsCoupledTo(Coupler coupler)
        {
            List<TrainCar> trainCarList = new List<TrainCar>();
            for (coupler = coupler.GetCoupled(); coupler != null; coupler = coupler.GetOppositeCoupler().GetCoupled())
                trainCarList.Add(coupler.train);
            return trainCarList;
        }
    }

    // reverser
    [HarmonyPatch(typeof(LocoControllerDiesel), "SetReverser")]
    class LocoControllerDiesel_SetReverser_Patch
    {
        static void Postfix(LocoControllerDiesel __instance, float position)
        {
            TrainCar currentCar = __instance.GetComponent<TrainCar>();
            TrainCar targetCar = null;
            Trainset trainset = null;

            if (Main.remoteCar)
            {
                targetCar = Main.remoteCar;
                trainset = targetCar.trainset;
            }
            else if (PlayerManager.Car != null)
            {
                targetCar = PlayerManager.Car;
                trainset = PlayerManager.Car.trainset;
            }

            if (currentCar == null || targetCar == null || !targetCar.Equals(currentCar) || trainset == null || trainset.cars.Count < 2)
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

                LocoControllerBase locoController = null;

                if (car.carType == TrainCarType.LocoShunter)
                {
                    locoController = car.GetComponent<LocoControllerShunter>();
                }
                else if (car.carType == TrainCarType.LocoDiesel)
                {
                    locoController = car.GetComponent<LocoControllerDiesel>();
                }

                if (locoController != null)
                {
                    if (GetCarsBehind(targetCar).Contains(car))
                    {
                        if (GetCarsInFrontOf(car).Contains(targetCar))
                        {
                            locoController.SetReverser(position);
                        }
                        else
                        {
                            locoController.SetReverser(position * -1f);
                        }
                    }
                    else if (GetCarsInFrontOf(targetCar).Contains(car))
                    {
                        if (GetCarsBehind(car).Contains(targetCar))
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

        public static List<TrainCar> GetCarsInFrontOf(TrainCar car)
        {
            return GetCarsCoupledTo(car.frontCoupler);
        }

        public static List<TrainCar> GetCarsBehind(TrainCar car)
        {
            return GetCarsCoupledTo(car.rearCoupler);
        }

        public static List<TrainCar> GetCarsCoupledTo(Coupler coupler)
        {
            List<TrainCar> trainCarList = new List<TrainCar>();
            for (coupler = coupler.GetCoupled(); coupler != null; coupler = coupler.GetOppositeCoupler().GetCoupled())
                trainCarList.Add(coupler.train);
            return trainCarList;
        }
    }

    // brake
    [HarmonyPatch(typeof(LocoControllerBase), "SetBrake")]
    class LocoControllerBase_SetBrake_Patch
    {
        static void Postfix(LocoControllerBase __instance, float nextTargetBrake)
        {
            TrainCar currentCar = __instance.GetComponent<TrainCar>();
            TrainCar targetCar = null;
            Trainset trainset = null;

            if (Main.remoteCar)
            {
                targetCar = Main.remoteCar;
                trainset = targetCar.trainset;
            }
            else if (PlayerManager.Car != null)
            {
                targetCar = PlayerManager.Car;
                trainset = PlayerManager.Car.trainset;
            }

            if (currentCar == null || targetCar == null || !targetCar.Equals(currentCar) || trainset == null || trainset.cars.Count < 2)
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
                        locoController.SetBrake(nextTargetBrake);
                    }
                }
                else if (car.carType == TrainCarType.LocoDiesel)
                {
                    LocoControllerDiesel locoController = car.GetComponent<LocoControllerDiesel>();

                    if (locoController)
                    {
                        locoController.SetBrake(nextTargetBrake);
                    }
                }
            }
        }
    }

    // independent brake
    [HarmonyPatch(typeof(LocoControllerBase), "SetIndependentBrake")]
    class LocoControllerBase_SetIndependentBrake_Patch
    {
        static void Postfix(LocoControllerBase __instance, float nextTargetIndependentBrake)
        {
            TrainCar currentCar = __instance.GetComponent<TrainCar>();
            TrainCar targetCar = null;
            Trainset trainset = null;

            if (Main.remoteCar)
            {
                targetCar = Main.remoteCar;
                trainset = targetCar.trainset;
            }
            else if (PlayerManager.Car != null)
            {
                targetCar = PlayerManager.Car;
                trainset = PlayerManager.Car.trainset;
            }

            if (currentCar == null || targetCar == null || !targetCar.Equals(currentCar) || trainset == null || trainset.cars.Count < 2)
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
                        locoController.SetIndependentBrake(nextTargetIndependentBrake);
                    }
                }
                else if (car.carType == TrainCarType.LocoDiesel)
                {
                    LocoControllerDiesel locoController = car.GetComponent<LocoControllerDiesel>();

                    if (locoController)
                    {
                        locoController.SetIndependentBrake(nextTargetIndependentBrake);
                    }
                }
            }
        }
    }

    // sand remote control
    [HarmonyPatch(typeof(LocoControllerShunter), "UpdateSand")]
    class LocoControllerShunter_UpdateSand_Patch
    {
        static void Prefix(LocoControllerShunter __instance, ToggleDirection toggle)
        {
            TrainCar targetCar = __instance.GetComponent<TrainCar>();
            Trainset trainset = targetCar.trainset;

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

                    if (locoController != null)
                    {
                        locoController.SetSandersOn(toggle == ToggleDirection.UP);
                    }
                } else if (car.carType == TrainCarType.LocoDiesel)
                {
                    LocoControllerDiesel locoController = car.GetComponent<LocoControllerDiesel>();

                    if (locoController != null)
                    {
                        locoController.SetSandersOn(toggle == ToggleDirection.UP);
                    }
                }
            }
        }
    }

    // sand remote control
    [HarmonyPatch(typeof(LocoControllerDiesel), "UpdateSand")]
    class LocoControllerDiesel_UpdateSand_Patch
    {
        static void Prefix(LocoControllerDiesel __instance, ToggleDirection toggle)
        {
            TrainCar targetCar = __instance.GetComponent<TrainCar>();
            Trainset trainset = targetCar.trainset;

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

                    if (locoController != null)
                    {
                        locoController.SetSandersOn(toggle == ToggleDirection.UP);
                    }
                }
                else if (car.carType == TrainCarType.LocoDiesel)
                {
                    LocoControllerDiesel locoController = car.GetComponent<LocoControllerDiesel>();

                    if (locoController != null)
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
                if (PlayerManager.Car == null || PlayerManager.Car.trainset == null) return;

                for (int i = 0; i < PlayerManager.Car.trainset.cars.Count; i++)
                {
                    TrainCar car = PlayerManager.Car.trainset.cars[i];

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
                    else if (car.carType == TrainCarType.LocoDiesel)
                    {
                        LocoControllerDiesel locoController = car.GetComponent<LocoControllerDiesel>();

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
                if (PlayerManager.Car == null || PlayerManager.Car.trainset == null) return;

                for (int i = 0; i < PlayerManager.Car.trainset.cars.Count; i++)
                {
                    TrainCar car = PlayerManager.Car.trainset.cars[i];

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
                    else if (car.carType == TrainCarType.LocoDiesel)
                    {
                        LocoControllerDiesel locoController = car.GetComponent<LocoControllerDiesel>();

                        if (locoController)
                        {
                            locoController.SetFan(e.newValue >= 0.5f);
                        }
                    }
                }
            });
        }
    }

    // sander & fan
    [HarmonyPatch(typeof(DieselDashboardControls), "OnEnable")]
    class DieselDashboardControls_OnEnable_Patch2
    {
        static DieselDashboardControls instance;

        static void Postfix(DieselDashboardControls __instance)
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
                if (PlayerManager.Car == null || PlayerManager.Car.trainset == null) return;

                for (int i = 0; i < PlayerManager.Car.trainset.cars.Count; i++)
                {
                    TrainCar car = PlayerManager.Car.trainset.cars[i];

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
                    else if (car.carType == TrainCarType.LocoDiesel)
                    {
                        LocoControllerDiesel locoController = car.GetComponent<LocoControllerDiesel>();

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
                if (PlayerManager.Car == null || PlayerManager.Car.trainset == null) return;

                for (int i = 0; i < PlayerManager.Car.trainset.cars.Count; i++)
                {
                    TrainCar car = PlayerManager.Car.trainset.cars[i];

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
                    else if (car.carType == TrainCarType.LocoDiesel)
                    {
                        LocoControllerDiesel locoController = car.GetComponent<LocoControllerDiesel>();

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
