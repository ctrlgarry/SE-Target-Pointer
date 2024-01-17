#region Prelude
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using VRageMath;
using VRage.Game;
using VRage.Collections;
using Sandbox.ModAPI.Ingame;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Game.EntityComponents;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;

// Change this namespace for each script you create.
//namespace SpaceEngineers.UWBlockPrograms.BatteryMonitor {


//Текст скрипта. Просто скопируйте его ctrl-c, ctrl-v прямо в программный блок. 
//КОНФИГУРАЦИЯ
double SCAN_DISTANCE = 1000;       //Дальность сканирования камеры-дальномера
string Camera_name = "Camera";     //Название камеры-дальномера
string Panel_name = "Cockpit [LCD]"; //Название блока (панели или кокпита), куда выводятся данные
int display_no = 0;   //число от 0 до 5 - номер дисплея, на который будет выводится информация. Для LCD панелей - 0
//----------------------
    
float PITCH = 0;
float YAW = 0;
private IMyCameraBlock camera;
private MyDetectedEntityInfo info;
private StringBuilder info_strings = new StringBuilder();
private IMyTextSurface textSurface; 

public Program(){
  // частота обновления информации: Update1 - раз в секунду, Update10 - раз в 10 сек и т.п.
  Runtime.UpdateFrequency = UpdateFrequency.Update1;
  List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
  GridTerminalSystem.GetBlocks(blocks);
  camera = GridTerminalSystem.GetBlockWithName(Camera_name) as IMyCameraBlock;
  camera.EnableRaycast = true;

  //если используется панель, а не кокпит, закомментарить 2 строки ниже и раскомментарить 2 строки еще ниже 
  IMyTerminalBlock cockpit = GridTerminalSystem.GetBlockWithName(Panel_name);
  textSurface = ((IMyTextSurfaceProvider)cockpit).GetSurface(display_no); 

  //IMyTextPanel lcd = GridTerminalSystem.GetBlockWithName(Panel_name) as IMyTextPanel;
  //textSurface = ((IMyTextSurfaceProvider)lcd).GetSurface(display_no); 

  textSurface.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;

}

public void Main(string args){
  if (camera.CanScan(SCAN_DISTANCE)) info = camera.Raycast(SCAN_DISTANCE, PITCH, YAW);
  info_strings.Clear();
  if (info.HitPosition.HasValue) {
    double dist = Vector3D.Distance(camera.GetPosition(), info.HitPosition.Value);
    info_strings.Append("Дист: " + dist.ToString("0.00")+"\r\n");
    info_strings.Append("Имя: " + info.Name + "\r\n"); 
    info_strings.Append("Тип: " + info.Type + "\r\n");
//    info_strings.Append("Скорость: " + info.Velocity.ToString("0.000") + "\r\n");
    info_strings.Append("Принадлежность: " + info.Relationship + "\r\n");
    info_strings.Append("Диапазон: " + camera.AvailableScanRange.ToString());
  }

  textSurface.WriteText(info_strings.ToString());
}
//конец скрипта