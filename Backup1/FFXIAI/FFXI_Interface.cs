using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FFACETools;

namespace FFXIAI
{
    using chat_line = FFACE.ChatTools.ChatLine;


    public static class Constants
    {
        // golfnsurf
        // from h1pp0 51568BF1XXXXXXXXE8
        /*
         * 18:21:59 <golfnsurf> ya, i got a sig tho
18:22:07 <golfnsurf> 8B 51 08 85 D2 74 18 3B 05
18:22:18 <golfnsurf> the 4 bytes after that sig is the pointer
18:22:43 <golfnsurf> it's another 2 layer pointer so follow that pointer then 
                     4c from the next pointer is menuindex
         */
        public const String MENU_SIG = "8B510885D274183B05";  //01b9XXXXXXXXe80419
        public const int MENU_OFFSET = 0; // 0x54;
        public const int MENU_INDEX_OFFSET = 0x4C;
        public const int MENU_LENGTH_OFFSET = 0x58;
    }

    ///////////////////// start FFACE wrapper /////////////////////////////////////////
    // ABSTRACTED FFACE CLASS
    public class FFXIInterface
    {

        // sigscan stuff
        [DllImport("SigScan.dll", EntryPoint = "InitializeSigScan")]
        public static extern void InitializeSigScan(uint iPID, [MarshalAs(UnmanagedType.LPStr)] string szModule);
        [DllImport("SigScan.dll", EntryPoint = "SigScan")]
        public static extern UInt32 SigScan([MarshalAs(UnmanagedType.LPStr)] string Pattern, int Offset);
        [DllImport("SigScan.dll", EntryPoint = "FinalizeSigScan")]
        public static extern void FinalizeSigScan();

        private int _thisProcess;
        private uint _menu_selection_sigscan;

        public uint get_menu_selection()
        {
            uint menu_selection_pointer = 0;
            _MemoryManager.read_int(_menu_selection_sigscan, ref menu_selection_pointer);
            uint menu_selection = 0;
            _MemoryManager.read_short(menu_selection_pointer + Constants.MENU_INDEX_OFFSET, ref menu_selection);
            return menu_selection;
        }

        public uint get_menu_length()
        {
            uint menu_selection_pointer = 0;
            _MemoryManager.read_int(_menu_selection_sigscan, ref menu_selection_pointer);
            uint menu_selection = 0;
            _MemoryManager.read_short(menu_selection_pointer + Constants.MENU_LENGTH_OFFSET, ref menu_selection);
            return menu_selection;
        }

        public void set_menu_selection(int index)
        {
            uint menu_selection_pointer = 0;
            _MemoryManager.read_int(_menu_selection_sigscan, ref menu_selection_pointer);
            byte[] Buffer = BitConverter.GetBytes(index);
            _MemoryManager.write_memory(menu_selection_pointer + Constants.MENU_INDEX_OFFSET, Buffer);

        }

        public void init_signature_scanning(uint pid)
        {
            InitializeSigScan((uint)pid, "FFXiMain.dll");
            _menu_selection_sigscan = SigScan(Constants.MENU_SIG, 0) + Constants.MENU_OFFSET;
            FinalizeSigScan();
        }

        public enum FishingStatus
        {
            STATUS_STANDING,
            STATUS_FISHING,
            STATUS_FISHBITE,
        };

        FFACE _FFACE { get; set; }
        MemoryManager _MemoryManager { get; set; }


        public FFXIInterface() { }
        public bool init(int pid)
        {
            _thisProcess = pid;
            //_baseAddress = 0;
            _FFACE = new FFACE(pid);
            _MemoryManager = new MemoryManager();
            _MemoryManager.init(pid);

            // personal stuff here:
            init_signature_scanning((uint)pid);
            return true;
        }

        public void Cleanup()
        {
            _MemoryManager.cleanup();
        }

        // @TODO clean this up/improve it
        public bool CheckAccess()
        {
            // removed auth because I'm not sure I care anymore.
            return true;
        }

        public void KillFish()
        {
            _FFACE.Fish.SetHP(0);
        }

        public string GetBaitName(int bait_id)
        {
            string item_name = "(no bait set)";
            try
            {
                item_name = FFACE.ParseResources.GetItemName(bait_id);
                return item_name;
            }
            catch (System.Exception ex)
            {
                return ex.ToString();
            }

        }

        public void EquipBait(int bait_id)
        {
            _FFACE.Windower.SendString("/equip ammo '" + GetBaitName(bait_id) + "'");
        }

        public string GetPlayerName()
        {
            return _FFACE.Player.Name;
        }

        public uint GetBaitCount(int bait_id)
        {
            if (bait_id > 0)
            {
                return _FFACE.Item.GetInventoryItemCount((ushort)bait_id);
            }
            else
            {
                return 0;
            }
        }

        public bool IsZoning()
        {
            return false;
            //return _FFACE.
        }

        public string GetZoneString()
        {
            if (_FFACE.Player != null)
                return _FFACE.Player.Zone.ToString();
            else
                return "";
        }

        public bool IsInventoryFull()
        {
            return (_FFACE.Item.InventoryCount == _FFACE.Item.InventoryMax);
        }

        public short GetInventoryCount()
        {
            return _FFACE.Item.InventoryCount;
        }

        public short GetInventoryMax()
        {
            return _FFACE.Item.InventoryMax;
        }

        public void SendString(string s)
        {
            _FFACE.Windower.SendString(s);
        }

        public void SendKeyCode(KeyCode k)
        {
            _FFACE.Windower.SendKeyPress(k);
        }

        public FishingStatus GetFishingStatus()
        {
            FFACETools.Status current_status = _FFACE.Player.Status;
            if (current_status == FFACETools.Status.Standing)
                return FishingStatus.STATUS_STANDING;
            if (current_status == FFACETools.Status.Fishing)
                return FishingStatus.STATUS_FISHING;
            if (current_status == FFACETools.Status.FishBite)
                return FishingStatus.STATUS_FISHBITE;

            return FishingStatus.STATUS_STANDING;
        }

        public string GetFishIDString()
        {


            int fish_id_1 = _FFACE.Fish.ID.ID1;
            int fish_id_2 = _FFACE.Fish.ID.ID2;
            int fish_id_3 = _FFACE.Fish.ID.ID3;
            int fish_id_4 = _FFACE.Fish.ID.ID4;

            return fish_id_1 + "-" + fish_id_2 + "-" + fish_id_3 + "-" + fish_id_4;
        }

        public uint GetTotalGil()
        {
            if (_FFACE.Item.GetInventoryItem(0) != null)
                return _FFACE.Item.GetInventoryItem(0).Count;
            else
                return 0;
        }
        public int GetFishMaxHP()
        {
            return _FFACE.Fish.HPMax;
        }

        public int GetFishCurrentHP()
        {
            return _FFACE.Fish.HPCurrent;
        }

        public void FightFish()
        {
            _FFACE.Fish.FightFish();
        }

        public int GetFishTimeOut()
        {
            return _FFACE.Fish.Timeout;
        }
        public void SetFishTimeOut(short x)
        {
            _FFACE.Fish.SetFishTimeOut(x);
        }

        public void PressEight()
        {
            _FFACE.Windower.SendKeyPress(KeyCode.NP_Number8);
        }

        public void PressEnter()
        {
            _FFACE.Windower.SendKeyPress(KeyCode.NP_EnterKey);
        }

        public void PressEscape()
        {
            _FFACE.Windower.SendKeyPress(KeyCode.EscapeKey);
        }

        public void PressTab()
        {
            _FFACE.Windower.SendKeyPress(KeyCode.TabKey);
        }

        public void PressDown()
        {
            _FFACE.Windower.SendKeyPress(KeyCode.DownArrow);
        }

        public void PressUp()
        {
            _FFACE.Windower.SendKeyPress(KeyCode.UpArrow);
        }

        public void PressLeft()
        {
            _FFACE.Windower.SendKeyPress(KeyCode.LeftArrow);
        }

        public void PressRight()
        {
            _FFACE.Windower.SendKeyPress(KeyCode.RightArrow);
        }


        // selling fish stuff;;
        public string GetTargetName()
        {
            return _FFACE.Target.Name;
        }

        public string GetSelectedDialogOption()
        {
            return "";
        }

        public string GetSelectedInventoryItem(bool use_resources)
        {
            if (use_resources)
            {
                return FFACE.ParseResources.GetItemName(_FFACE.Item.SelectedItemID);
            }
            else
            {
                return _FFACE.Item.SelectedItemName;
            }
            // 
        }

        public int GetSelectedInventoryIndex()
        {
            return _FFACE.Item.SelectedItemIndex;
        }

        public uint GetItemStackSize()
        {
            return _FFACE.Item.GetInventoryItemCountByIndex((byte)_FFACE.Item.SelectedItemIndex);
        }


        // 0.7.0
        public int GetLineCount()
        {
            return _FFACE.Chat.GetLineCount;
        }

        public chat_line GetNextChatLine()
        {
            return _FFACE.Chat.GetNextLine();
        }

        public int GetFishingRodId()
        {
            int i = _FFACE.Item.GetEquippedItemID(EquipSlot.Range);
            return i;
        }

        public int GetBaitId()
        {
            int i = _FFACE.Item.GetEquippedItemID(EquipSlot.Ammo);
            return i;
        }

        public string GetCurrentTimeString()
        {
            // Update display stats here
            FFACE.TimerTools.VanaTime current_vana_time = _FFACE.Timer.GetVanaTime();
            string time = current_vana_time.GetDayOfWeekName(current_vana_time.DayType) + " " +
                current_vana_time.Hour.ToString("00") + ":" + current_vana_time.Minute.ToString("00");
            string moon_info = current_vana_time.GetMoonPhaseName(current_vana_time.MoonPhase) + " Moon " + current_vana_time.MoonPercent.ToString();
            return "Time: " + time + " " + moon_info + "%";
        }

        // returns the number of seconds before the ferry docks in the current zone
        //         public int GetTimeTillFerry(string zone, bool departing)
        //         {
        //             FFACE.TimerTools.VanaTime current_vana_time = _FFACE.Timer.GetVanaTime();
        //             byte curr_hour  = current_vana_time.Hour;
        //             byte curr_min   = current_vana_time.Minute;
        // 
        //             switch (zone)
        //             {
        //             case "Mhaura":
        //                 {
        //                     if (departing)
        //                     {
        // 
        //                     }
        //                     else
        //                     {
        // 
        //                     }
        // 
        //                 }
        // 
        //             	break;
        //             }
        //         }

        public bool IsSelbinaFerryDocked()
        {
            FFACE.TimerTools.VanaTime current_vana_time = _FFACE.Timer.GetVanaTime();

            if ((current_vana_time.Hour == 22 ||
                current_vana_time.Hour == 6 ||
                current_vana_time.Hour == 14) &&
                current_vana_time.Minute > 40)
            {
                return true; // we're docked
            }
            else if ((current_vana_time.Hour > 22 && current_vana_time.Hour < 24) ||
                (current_vana_time.Hour > 6 && current_vana_time.Hour < 8) ||
                (current_vana_time.Hour > 14 && current_vana_time.Hour < 16))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool CompareCoords(float c1, float c2)
        {
            if (Math.Abs(c1 - c2) <= 0 /*_FFACE.Navigator.DistanceTolerance*/)
            {
                //Debugger.Log(0, null, "Diff: " + Math.Abs(c1 - c2) + "\n");/
                return true;
            }


            return false;
        }

        public void StopRunning()
        {
            _FFACE.Navigator.Reset();
        }

        public void setup_nav_options()
        {
            _FFACE.Navigator.HeadingTolerance = 40;
            _FFACE.Navigator.DistanceTolerance = 2.0f;
            _FFACE.Navigator.UseArrowKeysForTurning = true;
            _FFACE.Navigator.GotoDelay = 40;
            _FFACE.Navigator.SpeedDelay = 40;
        }

        public void GoToCoord(float x, float z, bool keepRunning, int timeout)
        {
            FFACE.NavigatorTools.dPoint dx, dz;
            dx = delegate { return x; };
            dz = delegate { return z; };

            _FFACE.Navigator.FaceHeading(x, z);
            _FFACE.Navigator.Goto(dx, dz, keepRunning, timeout);

        }

        public void GoToTarget()
        {
            _FFACE.Navigator.GotoTarget(250);
        }

        public void SetThirdPerson()
        {
            _FFACE.Navigator.SetViewMode(ViewMode.ThirdPerson);
        }

        public double GetDistanceToTarget()
        {
            return _FFACE.Navigator.DistanceTo(_FFACE.Target.PosX, _FFACE.Target.PosZ);
        }

        public Single GetHeading()
        {
            return _FFACE.Player.PosH;
        }

        public void SetHeading(float head)
        {
            _FFACE.Navigator.FaceHeading(head, HeadingType.Radians);
        }


        public double GetDistanceToPoint(float x, float z)
        {
            // Debugger.Log(0, null, "Distance: " + _FFACE.Navigator.DistanceTo(x, z) + "\n");
            return _FFACE.Navigator.DistanceTo(x, z);
        }

        public System.Windows.Media.Media3D.Vector3D GetPOS()
        {
            System.Windows.Media.Media3D.Vector3D v = new System.Windows.Media.Media3D.Vector3D();
            v.X = _FFACE.Player.PosX;
            v.Y = _FFACE.Player.PosY;
            v.Z = _FFACE.Player.PosZ;

            return v;
        }

        public float GetPOSX()
        {
            return _FFACE.Player.PosX;
        }

        public float GetPOSY()
        {
            return _FFACE.Player.PosY;
        }

        public float GetPOSZ()
        {
            return _FFACE.Player.PosZ;
        }

        public bool CheckMobArrayForGM()
        {
            //  Zone z1 = _FFACE.Search.Zone(0);
            // MessageBox.Show("First person in the list is in " + z1.ToString("G"));
            return false;
        }
        //         public string GetCurrentWeatherString()
        //         {
        //             //@TODO
        //         }


        ///////////////////// end FFACE wrapper /////////////////////////////////////////

    }

}