using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace VehiclesMetaEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string originalFileContent = "";
        string originalFileName = "";
        XmlNodeList listVehicles;
        readonly XmlDocument doc = new();
        XDocument docVeh = new();
        readonly string[] flagsList = {
            "FLAG__SMALL_WORKER","",
            "FLAG__BIG","Vehicle is large.",
            "FLAG__NO_BOOT","Vehicle has no boot.",
            "FLAG__ONLY_DURING_OFFICE_HOURS","Vehicle Will only spawn during office hours (06:00 - 20:00?) Flag is unused but is derivative from the IV flag onlyduringofficehours",
            "FLAG__BOOT_IN_FRONT","The boot of the vehicle is in the front.",
            "FLAG__IS_VAN","Vehicle is a van.",
            "FLAG__AVOID_TURNS","AI driving the vehicle will try to avoid turning at intersections, unless no other path is available.",
            "FLAG__HAS_LIVERY","The vehicle will cycle between liveries (typically named modelname_sign_x) located within the vehicle's texture dictionary and applied to diffuse2 texture of vehicle_paint3 shader.",
            "FLAG__LIVERY_MATCH_EXTRA","",
            "FLAG__SPORTS","",
            "FLAG__DELIVERY","",
            "0xB5A93F62","",
            "FLAG__ONLY_ON_HIGHWAYS","Vehicle Will only spawn on highways. Flag is unused but is derivative from the IV flag onlyonhighways.",
            "FLAG__TALL_SHIP","",
            "FLAG__SPAWN_ON_TRAILER","The vehicle can randomly spawn on the \"tr2\" trailer in traffic.",
            "FLAG__SPAWN_BOAT_ON_TRAILER","The boat can randomly spawn on the \"boattrailer\" trailer in traffic.",
            "FLAG__EXTRAS_GANG","The vehicle spawns with extras 5-9 if driven by a ped defined as a gang member.",
            "FLAG__EXTRAS_CONVERTIBLE","The vehicle cycles between extras 1-3 at random. If extra1 is toggled on and extras 2 and 3 are toggled off, the front left and front right windows will be removed.",
            "FLAG__EXTRAS_TAXI","The vehicle cycles between extras 5, 6 and 9 at random.",
            "FLAG__EXTRAS_RARE","The vehicle can spawn with extras toggle on, but rarely.",
            "FLAG__EXTRAS_REQUIRE","The vehicle always spawns with at least 1 extra toggled on.",
            "FLAG__EXTRAS_STRONG","Prevents extras from breaking off during collisions.",
            "FLAG__EXTRAS_ONLY_BREAK_WHEN_DESTROYED","Extras on the vehicle can only break off once the vehicle is completely destroyed, eg. by explosions.",
            "FLAG__EXTRAS_SCRIPT","The vehicle has extras that will never spawn at random and must be toggled on through script.",
            "FLAG__EXTRAS_ALL","The vehicle always spawns with all extras toggled on.",
            "FLAG__EXTRAS_MATCH_LIVERY","",
            "FLAG__DONT_ROTATE_TAIL_ROTOR","Prevents the helicopter's tail rotor from rotating.",
            "FLAG__PARKING_SENSORS","",
            "FLAG__PEDS_CAN_STAND_ON_TOP","Allows the player and AI peds to stand on top of the vehicle without ragdolling off.",
            "0x77C9F804","",
            "FLAG__GEN_NAVMESH","Generates navmesh on the vehicle. Typically used on boats, trailers, and train cars.",
            "FLAG__LAW_ENFORCEMENT","",
            "FLAG__EMERGENCY_SERVICE","",
            "FLAG__DRIVER_NO_DRIVE_BY","Prevents the driver from using any weapons in the vehicle.",
            "FLAG__NO_RESPRAY","Removes the option to respray the vehicle in Los Santos Customs.",
            "FLAG__IGNORE_ON_SIDE_CHECK","",
            "FLAG__RICH_CAR","Rich NPCs may be seen driving the vehicle in traffic.",
            "FLAG__AVERAGE_CAR","Average NPCs may be seen driving the vehicle in traffic.",
            "FLAG__POOR_CAR","Poor NPCs may be seen driving the vehicle in traffic.",
            "FLAG__ALLOWS_RAPPEL","Allows the player and AI peds to rappel from the vehicle.",
            "FLAG__DONT_CLOSE_DOOR_UPON_EXIT","The player and AI peds won't close the door after exiting the vehicle.",
            "FLAG__USE_HIGHER_DOOR_TORQUE","",
            "FLAG__DISABLE_THROUGH_WINDSCREEN","Prevents the player and AI peds from being ejected through the windshield during a crash.",
            "FLAG__IS_ELECTRIC","",
            "FLAG__NO_BROKEN_DOWN_SCENARIO","The vehicle will never spawn as part of a WORLD_VEHICLE_BROKEN_DOWN scenario.",
            "FLAG__IS_JETSKI","",
            "FLAG__DAMPEN_STICKBOMB_DAMAGE","The vehicle takes less damage from explosions caused by a sticky bomb.",
            "FLAG__DONT_SPAWN_IN_CARGEN","The vehicle will never spawn during world car generation.",
            "FLAG__IS_OFFROAD_VEHICLE","",
            "FLAG__INCREASE_PED_COMMENTS","Causes AI peds to randomly comment on the vehicle more frequently.",
            "FLAG__EXPLODE_ON_CONTACT","",
            "FLAG__USE_FAT_INTERIOR_LIGHT","Increases the size and intensity of the vehicle's interior light.",
            "FLAG__HEADLIGHTS_USE_ACTUAL_BONE_POS","",
            "FLAG__FAKE_EXTRALIGHTS","Causes extralights to behave as normal headlights.",
            "FLAG__CANNOT_BE_MODDED","Prevents the vehicle from entering Los Santos Customs.",
            "FLAG__DONT_SPAWN_AS_AMBIENT","Prevents the vehicle from spawning in ambient scenarios.",
            "FLAG__IS_BULKY","",
            "FLAG__BLOCK_FROM_ATTRACTOR_SCENARIO","The vehicle will never spawn as part of a WORLD_VEHICLE_ATTRACTOR scenario.",
            "FLAG__IS_BUS","",
            "FLAG__USE_STEERING_PARAM_FOR_LEAN","",
            "FLAG__CANNOT_BE_DRIVEN_BY_PLAYER","Prevents the player from entering the vehicle.",
            "FLAG__SPRAY_PETROL_BEFORE_EXPLOSION","The vehicle will spill gasoline before exploding.",
            "FLAG__ATTACH_TRAILER_ON_HIGHWAY","The vehicle will spawns with a trailer only on roads marked as highways.",
            "FLAG__ATTACH_TRAILER_IN_CITY","The vehicle will spawns with a trailer only in the Los Santos region.",
            "FLAG__HAS_NO_ROOF","",
            "FLAG__ALLOW_TARGETING_OF_OCCUPANTS","",
            "FLAG__RECESSED_HEADLIGHT_CORONAS","",
            "FLAG__RECESSED_TAILLIGHT_CORONAS","",
            "FLAG__IS_TRACKED_FOR_TRAILS","",
            "FLAG__HEADLIGHTS_ON_LANDINGGEAR","",
            "FLAG__CONSIDERED_FOR_VEHICLE_ENTRY_WHEN_STOOD_ON","The player and AI peds will prioritize entering the vehicle when standing on top of it.",
            "FLAG__GIVE_SCUBA_GEAR_ON_EXIT","The player receives scuba gear when exiting the vehicle.",
            "FLAG__IS_DIGGER","",
            "FLAG__IS_TANK","Makes the vehicle fireproof. AI peds will react to it as they would towards a tank.",
            "FLAG__USE_COVERBOUND_INFO_FOR_COVERGEN","",
            "FLAG__CAN_BE_DRIVEN_ON","Allows other vehicles to drive on top of the vehicle.",
            "FLAG__HAS_BULLETPROOF_GLASS","Prevents the glass from breaking, essentially making it behave as static glass found on buildings around the map.",
            "FLAG__CANNOT_TAKE_COVER_WHEN_STOOD_ON","Prevents the player and AI peds from taking cover while standing on top of the vehicle.",
            "FLAG__INTERIOR_BLOCKED_BY_BOOT","",
            "FLAG__DONT_TIMESLICE_WHEELS","Prevents the vehicle wheels from producing the time slicing effect when they rotate at higher speed.",
            "FLAG__FLEE_FROM_COMBAT","The AI driver of the vehicle will always drive away from the area if the player or AI is currently in combat in that area.",
            "FLAG__DRIVER_SHOULD_BE_FEMALE","The vehicle will only spawn driven by female peds.",
            "FLAG__DRIVER_SHOULD_BE_MALE","The vehicle will only spawn driven by male peds.",
            "FLAG__COUNT_AS_FACEBOOK_DRIVEN","Used to track progress of the vehicles the player purchased from Legendary Motorsport and drove. Once all vehicles with that flag were driven the game would send a post to Facebook, if linked. The flag is unused on PS4, PC and XB1 version of the game due to the removal of Facebook integration.",
            "FLAG__BIKE_CLAMP_PICKUP_LEAN_RATE","",
            "FLAG__PLANE_WEAR_ALTERNATIVE_HELMET","",
            "FLAG__USE_STRICTER_EXIT_COLLISION_TESTS","",
            "FLAG__TWO_DOORS_ONE_SEAT","",
            "FLAG__USE_LIGHTING_INTERIOR_OVERRIDE","Keeps interiorlight node active, even when vehicle engine is off/player is not inside.",
            "FLAG__USE_RESTRICTED_DRIVEBY_HEIGHT","Restricts how high the player can aim their weapon inside the vehicle.",
            "FLAG__CAN_HONK_WHEN_FLEEING","The AI peds will use the horn when trying to flee the area.",
            "FLAG__PEDS_INSIDE_CAN_BE_SET_ON_FIRE_MP","The player and AI peds can be set on fire while they're inside the vehicle. Works only in GTA Online. Added in b395 (X360/PS3).",
            "FLAG__REPORT_CRIME_IF_STANDING_ON","The player will gain a wanted level if they stand on top of the vehicle. Added in b554 (X360/PS3).",
            "FLAG__HELI_USES_FIXUPS_ON_OPEN_DOOR","Added in b580 (X360/PS3).",
            "FLAG__FORCE_ENABLE_CHASSIS_COLLISION","",
            "FLAG__CANNOT_BE_PICKUP_BY_CARGOBOB","Prevents the vehicle from being picked up by a Cargobob. Added in b760 (X360/PS3).",
            "FLAG__CAN_HAVE_NEONS","The vehicle has a chance of spawning in traffic with neon lights equipped. Added in the PS4, PC, and XB1 release of the game.",
            "FLAG__HAS_INTERIOR_EXTRAS","The vehicle can spawn with extras ten, 11, and 12 at random. Added in the PS4, PC, and XB1 release of the game.",
            "FLAG__HAS_TURRET_SEAT_ON_VEHICLE","Added in b760 (X360/PS3).",
            "FLAG__ALLOW_OBJECT_LOW_LOD_COLLISION","Added in the PS4, PC, and XB1 release of the game.",
            "FLAG__DISABLE_AUTO_VAULT_ON_VEHICLE","Prevents the player and AI peds from automatically climbing on top of the vehicle. Added in b760 (X360/PS3).",
            "FLAG__USE_TURRET_RELATIVE_AIM_CALCULATION","Added in b760 (X360/PS3).",
            "FLAG__USE_FULL_ANIMS_FOR_MP_WARP_ENTRY_POINTS","Added in b760 (X360/PS3).",
            "FLAG__HAS_DIRECTIONAL_SHUFFLES","Added in b760 (X360/PS3).",
            "FLAG__DISABLE_WEAPON_WHEEL_IN_FIRST_PERSON","Disables the weapon wheel in first person view. Added in the PS4, PC, and XB1 release of the game.",
            "FLAG__USE_PILOT_HELMET","Forces the player and AI peds to put on a pilot helmet when inside the vehicle. Added in the PS4, PC, and XB1 release of the game.",
            "FLAG__USE_WEAPON_WHEEL_WITHOUT_HELMET","Added in the PS4, PC, and XB1 release of the game.",
            "FLAG__PREFER_ENTER_TURRET_AFTER_DRIVER","Added in b760 (X360/PS3).",
            "FLAG__USE_SMALLER_OPEN_DOOR_RATIO_TOLERANCE","Added in b760 (X360/PS3).",
            "FLAG__USE_HEADING_ONLY_IN_TURRET_MATRIX","Added in b760 (X360/PS3).",
            "FLAG__DONT_STOP_WHEN_GOING_TO_CLIMB_UP_POINT","Added in b760 (X360/PS3).",
            "FLAG__HAS_REAR_MOUNTED_TURRET","Added in b760 (X360/PS3).",
            "FLAG__DISABLE_BUSTING","Added in b760 (X360/PS3).",
            "FLAG__IGNORE_RWINDOW_COLLISION","Added in b760 (X360/PS3).",
            "FLAG__HAS_GULL_WING_DOORS","The vehicle's doors will open upwards, as gull wing doors would. Added in b372.",
            "FLAG__CARGOBOB_HOOK_UP_CHASSIS","Added in b372.",
            "FLAG__USE_FIVE_ANIM_THROW_FP","Added in the PS4, PC, and XB1 release of the game.",
            "FLAG__ALLOW_HATS_NO_ROOF","Allows the player and AI peds to keep wearing their hats if the vehicle has no roof. Added in the PS4, PC, and XB1 release of the game.",
            "FLAG__HAS_REAR_SEAT_ACTIVITIES","The vehicle has activities in the rear seat. Works only in GTA Online. Added in b372.",
            "FLAG__HAS_LOWRIDER_HYDRAULICS","The vehicle is equipped with lowrider hydraulics. Added in b393.",
            "FLAG__HAS_BULLET_RESISTANT_GLASS","The vehicle's glass becomes resistant to bullets. The collision ID for windows must be set to ID 122 (strong glass/windshield) for the flag to have an effect. Added in b573.",
            "FLAG__HAS_INCREASED_RAMMING_FORCE","The vehicle is able to ram vehicles out the way more easily. Added in b573.",
            "FLAG__HAS_CAPPED_EXPLOSION_DAMAGE","The vehicle takes less damage from explosions. Added in b573.",
            "FLAG__HAS_LOWRIDER_DONK_HYDRAULICS","The vehicle is equipped with lowrider donk hydraulics. The difference is that the vehicle cannot \"hop\" and only shifts in the direction the hydraulics are used. Added in b678.",
            "FLAG__HELICOPTER_WITH_LANDING_GEAR","The helicopter has a landing gear. Added in b757.",
            "FLAG__JUMPING_CAR","The vehicle is able to jump. The jump force can be modified through handling files. Added in b944.",
            "FLAG__HAS_ROCKET_BOOST","The vehicle can use a rocket boost. The power and duration can be modified through handling files. Added in b944.",
            "FLAG__RAMMING_SCOOP","The vehicle has a scoop that can push vehicles out the way. Added in b944.",
            "FLAG__HAS_PARACHUTE","The vehicle is equipped with a parachute that can be enabled while mid-air. Added in b944.",
            "FLAG__RAMP","The vehicle is able to eject other vehicles while driving under them. Added in b944.",
            "FLAG__HAS_EXTRA_SHUFFLE_SEAT_ON_VEHICLE","Added in b944.",
            "FLAG__FRONT_BOOT","Allows the \"boot\" bone to fall off at high speeds when opened, similarly to how the \"bonnet\" bone does. Added in b944.",
            "FLAG__HALF_TRACK","Added in b1011.",
            "FLAG__RESET_TURRET_SEAT_HEADING","Added in b1011.",
            "FLAG__TURRET_MODS_ON_ROOF","Added in b1103.",
            "FLAG__UPDATE_WEAPON_BATTERY_BONES","Added in b1103.",
            "FLAG__DONT_HOLD_LOW_GEARS_WHEN_ENGINE_UNDER_LOAD","Added in b1103.",
            "FLAG__HAS_GLIDER","Added in b1103.",
            "FLAG__INCREASE_LOW_SPEED_TORQUE","Added in b1103.",
            "FLAG__USE_AIRCRAFT_STYLE_WEAPON_TARGETING","Added in b1103.",
            "FLAG__KEEP_ALL_TURRETS_SYNCHRONISED","Added in b1103.",
            "FLAG__SET_WANTED_FOR_ATTACHED_VEH","Added in b1103.",
            "FLAG__TURRET_ENTRY_ATTACH_TO_DRIVER_SEAT","Added in b1180.",
            "FLAG__USE_STANDARD_FLIGHT_HELMET","Added in b1180.",
            "FLAG__SECOND_TURRET_MOD","Added in b1180.",
            "FLAG__THIRD_TURRET_MOD","Added in b1180.",
            "FLAG__HAS_EJECTOR_SEATS","Added in b1180.",
            "0x2028D687","Added in b1180.",
            "FLAG__HAS_JATO_BOOST_MOD","The aircraft has a JATO boost modification. The JATO boost is hardcoded to the VMT_EXHAUST category. Added in b1180.",
            "FLAG__IGNORE_TRAPPED_HULL_CHECK","Added in b1180.",
            "FLAG__HOLD_TO_SHUFFLE","The player will shuffle seats when holding the context button. Added in b1180.",
            "FLAG__TURRET_MOD_WITH_NO_STOCK_TURRET","Added in b1180.",
            "FLAG__EQUIP_UNARMED_ON_ENTER","The player will equip the \"Unarmed\" weapon when entering the vehicle. Added in b1180.",
            "FLAG__DISABLE_CAMERA_PUSH_BEYOND","Added in b1180.",
            "FLAG__HAS_VERTICAL_FLIGHT_MODE","The vehicle has vertical takeoff and flight ability. Unlike the standard VTOL ability, the vehicle cannot switch to flight mode while landed. Added in b1290.",
            "FLAG__HAS_OUTRIGGER_LEGS","The vehicle has outrigger legs. The vehicle is unable to drive while the legs are extended. Added in b1290.",
            "FLAG__CAN_NAVIGATE_TO_ON_VEHICLE_ENTRY","Added in b1290.",
            "FLAG__DROP_SUSPENSION_WHEN_STOPPED","The vehicle has air suspension. The suspension raises while driving and drops when the vehicle is stopped. The default suspension height in handling MUST be the same as when driving normally, as the flag only drops the suspension. Added in b1290.",
            "FLAG__DONT_CRASH_ABANDONED_NEAR_GROUND","Added in b1290.",
            "FLAG__USE_INTERIOR_RED_LIGHT","The vehicle's interior light will be colored red. Added in b1290.",
            "FLAG__HAS_HELI_STRAFE_MODE","The aircraft is able to strafe like a helicopter. Added in b1290.",
            "FLAG__HAS_VERTICAL_ROCKET_BOOST","The rocket or JATO boost will propel the vehicle directly upwards. Added in b1290.",
            "FLAG__CREATE_WEAPON_MANAGER_ON_SPAWN","Added in b1290.",
            "FLAG__USE_ROOT_AS_BASE_LOCKON_POS","Added in b1290.",
            "FLAG__HEADLIGHTS_ON_TAP_ONLY","Added in b1290.",
            "FLAG__CHECK_WARP_TASK_FLAG__DURING_ENTER","Added in b1290.",
            "FLAG__USE_RESTRICTED_DRIVEBY_HEIGHT_HIGH","Restricts how high the player can aim their weapon inside the vehicle. The limit is higher than the default flag. Added in b1290.",
            "FLAG__INCREASE_CAMBER_WITH_SUSPENSION_MOD","Increases the vehicle camber when suspension mods are equipped. Added in b1365.",
            "FLAG__NO_HEAVY_BRAKE_ANIMATION","The player and AI peds will not play a \"lean forward\" animation when breaking heavily. Added in b1365.",
            "FLAG__HAS_TWO_BONNET_BONES","The vehicle will use the \"bonnet\" and \"cargodoor\" bones simultaneously when opening the hood. Added in b1493.",
            "FLAG__DONT_LINK_BOOT2","Added in b1493.",
            "FLAG__HAS_INCREASED_RAMMING_FORCE_WITH_CHASSIS_MOD","Chassis modification apply more ramming force to the vehicle. Added in b1493.",
            "0x4C8630D9","Added in b1604.",
            "FLAG__HAS_EXTENDED_COLLISION_MODS","Added in b1604.",
            "FLAG__HAS_NITROUS_MOD","The vehicle has modifications that enable nitro boost. The nitro mod boost is hardcoded to the VMT_ENGINEBAY2 category. The nitro boost can also be manually enabled and forced on the vehicle without needing this flag by scripts. Added in b1604.",
            "FLAG__HAS_JUMP_MOD","The vehicle has a jump boost modification. The jump boost modification is hardcoded to the VMT_ENGINEBAY3 category. Added in b1604.",
            "FLAG__HAS_RAMMING_SCOOP_MOD","The vehicle has a ramming scoop modification. The ramming scoop modification is hardcoded to the VMT_CHASSIS2 category and require a scoop_1mod, scoop_2mod, and scoop_3mod collision bones. Added in b1604.",
            "FLAG__HAS_SUPER_BRAKES_MOD","The vehicle has a super brakes modification. Added in b1604.",
            "FLAG__CRUSHES_OTHER_VEHICLES","The vehicle does more damage to any vehicle it drives on top of. Added in b1604.",
            "FLAG__HAS_WEAPON_BLADE_MODS","The vehicle has sawblade modifications. The player and AI peds will be killed on contact with the sawblade collision. Added in b1604.",
            "FLAG__HAS_WEAPON_SPIKE_MODS","The vehicle has spike modifications. The player and AI peds will be killed if they run into the spike collision. Added in b1604.",
            "FLAG__FORCE_BONNET_CAMERA_INSTEAD_OF_POV","Added in b1604.",
            "FLAG__RAMP_MOD","The vehicle has a ramp modification. Added in b1604.",
            "FLAG__HAS_TOMBSTONE","The vehicle has a very heavy and solid 'tombstone' part that can be ejected to stop pursuing vehicles. Added in b1604.",
            "FLAG__HAS_SIDE_SHUNT","The vehicle is able to shunt vehicles out the way. Added in b1604.",
            "FLAG__HAS_FRONT_SPIKE_MOD","The vehicle has a front spike modification. Added in b1604.",
            "FLAG__HAS_RAMMING_BAR_MOD","The vehicle has a ramming bar modification. The ramming bar mod is hardcoded to the VMT_CHASSIS2 category and require a ram_1mod, ram_2mod, and ram_3mod collision bones. Added in b1604.",
            "FLAG__TURRET_MODS_ON_CHASSIS5","Enables turret mods on the VMT_CHASSIS5 category, which normally are hardcoded to the VMT_ROOF category. Added in b1604.",
            "FLAG__HAS_SUPERCHARGER","Permanently enables the vacuum and blower/boost dials on the vehicle. Added in b1604.",
            "FLAG__IS_TANK_WITH_FLAME_DAMAGE","Similar to FLAG__IS_TANK, except the vehicle is not fireproof. Added in b1604.",
            "FLAG__DISABLE_DEFORMATION","Disables visual deformation on the vehicle. Added in b1604.",
            "FLAG__ALLOW_RAPPEL_AI_ONLY","Added in b1734.",
            "FLAG__USE_RESTRICTED_DRIVEBY_HEIGHT_MID_ONLY","Added in b1734.",
            "FLAG__FORCE_AUTO_VAULT_ON_VEHICLE_WHEN_STUCK","Added in b1734.",
            "FLAG__SPOILER_MOD_DOESNT_INCREASE_GRIP","The vehicle's spoiler mods do not add traction. Added in b1868.",
            "FLAG__NO_REVERSING_ANIMATION","The player and AI peds do not play an \"look back\" animation when reversing the vehicle. Added in b1868.",
            "FLAG__IS_QUADBIKE_USING_BIKE_ANIMATIONS","Allows the quadbike vehicle type to use bike layouts and animations. Added in b1868.",
            "FLAG__IS_FORMULA_VEHICLE","Changes the tire mark texture from standard tires to slick tires. Added in b2060.",
            "FLAG__LATCH_ALL_JOINTS","Added in b2189.",
            "FLAG__REJECT_ENTRY_TO_VEHICLE_WHEN_STOOD_ON","If the player is standing on top of the vehicle, the player character will not attempt to pathfind to the nearest vehicle door to enter and will not warp inside the vehicle. Added in b2189.",
            "FLAG__CHECK_IF_DRIVER_SEAT_IS_CLOSER_THAN_TURRETS_WITH_ON_BOARD_ENTER","Checks whether the player character is closer to the driver's seat than the turret seat before deciding on which one to pathfind to. Added in b2189.",
            "FLAG__RENDER_WHEELS_WITH_ZERO_COMPRESSION","Prevents the wheels and suspension from visually moving. Added in b2189.",
            "FLAG__USE_LENGTH_OF_VEHICLE_BOUNDS_FOR_PLAYER_LOCKON_POS","Added in b2189.",
            "FLAG__PREFER_FRONT_SEAT","Added in b2372." 
        };

        public MainWindow()
        {
            InitializeComponent();

            for (var flag = 0; flag < flagsList.Length / 2; flag++)
            {
                Grid gridFlag = new();
                ColumnDefinition c1 = new()
                {
                    Width = new GridLength(0, GridUnitType.Auto)
                };
                ColumnDefinition c2 = new()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                };
                gridFlag.ColumnDefinitions.Add(c1);
                gridFlag.ColumnDefinitions.Add(c2);
                ListFlags.Children.Add(gridFlag);
                CheckBox flagName = new()
                {
                    Content = flagsList[flag * 2],
                    VerticalAlignment = VerticalAlignment.Center
                };
                flagName.SetValue(Grid.ColumnProperty, 0);
                gridFlag.Children.Add(flagName);
                Label flagDesc = new()
                {
                    Content = flagsList[flag * 2 + 1],
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CC000000")),
                    FontStyle = FontStyles.Italic
                };
                flagDesc.SetValue(Grid.ColumnProperty, 1);
                gridFlag.Children.Add(flagDesc);
            }
        }

        private void OpenFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Meta files (*.meta)|*.meta|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                originalFileContent = File.ReadAllText(openFileDialog.FileName);
                originalFileName = openFileDialog.FileName;

                LoadFile(originalFileContent);

                LoadFileOverlay.Visibility = Visibility.Collapsed;
            }
        }

        private void SaveFileBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                FileName = originalFileName,
                Filter = "Meta files (*.meta)|*.meta|All files (*.*)|*.*"
            };
            if (saveFileDialog.ShowDialog() == true && saveFileDialog.FileName != null)
            {
                if (!File.Exists(saveFileDialog.FileName))
                {
                    File.WriteAllText(originalFileName + ".OLD", originalFileContent);
                    File.WriteAllText(saveFileDialog.FileName, FilePreview.Text);

                    MessageBox.Show("Le fichier original ('OLD') a été sauvegardé au même emplacement.", "Fichier original sauvegardé", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else { File.WriteAllText(saveFileDialog.FileName, FilePreview.Text); }
            }
        }

        private void LoadFile(string file, int index = -1)
        {
            string vehname;
            bool stopLoadingFile = false;
            try
            {
                doc.LoadXml(file);
                listVehicles = doc.ChildNodes[1].ChildNodes[2].ChildNodes;
            }
            catch (Exception error)
            {
                MessageBox.Show("Fichier incompatible\r\n\r\nDétails :\r\n" + error, "Impossible d'ouvrir le fichier", MessageBoxButton.OK, MessageBoxImage.Warning);
                stopLoadingFile = true;
                throw;
            }

            if (!stopLoadingFile)
            {
                FilePreview.Text = originalFileContent;

                vehicleSelector.Items.Clear();
                for (var vehnb = 0; vehnb < listVehicles.Count; vehnb++)
                {
                    vehname = listVehicles[vehnb].ChildNodes[0].InnerText;
                    ComboBoxItem item = new()
                    {
                        Content = vehname
                    };
                    vehicleSelector.Items.Add(item);
                }

                menuSaveItem.IsEnabled = true;
                DataGrid.IsEnabled = true;
                TabControlPreviews.IsEnabled = true;

                VehicleSelector_SelectionChanged(this, null);
            }
        }

        int indexCurrVeh;
        string modelNameOrig = "";
        string handlingIdOrig = "";
        string gameNameOrig = "";
        string makeNameOrig = "";
        string audioHashOrig = "";
        string layoutOrig = "";
        string typeOrig = "";
        string classOrig = "";
        string diffuseTintOrig = "";
        string vehicleLOD1Orig = "";
        string vehicleLOD2Orig = "";
        string vehicleLOD3Orig = "";
        string vehicleLOD4Orig = "";
        string vehicleLOD5Orig = "";
        string vehicleLOD6Orig = "";
        string[] flagsOrig ;

        private void VehicleSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (vehicleSelector.SelectedIndex != -1)
                indexCurrVeh = vehicleSelector.SelectedIndex;
            else
            {
                indexCurrVeh = 0;
                vehicleSelector.SelectedIndex = 0;
            }

            docVeh = XDocument.Parse("<Item>" + listVehicles[indexCurrVeh].InnerXml + "</Item>");
            VehiclePreview.Text = docVeh.ToString();
            TabControlPreviews.SelectedIndex = 0;

            modelNameOrig = doc.ChildNodes[1].ChildNodes[2].ChildNodes[indexCurrVeh].SelectSingleNode(".//modelName").InnerText;
            handlingIdOrig = doc.ChildNodes[1].ChildNodes[2].ChildNodes[indexCurrVeh].SelectSingleNode(".//handlingId").InnerText;
            gameNameOrig = doc.ChildNodes[1].ChildNodes[2].ChildNodes[indexCurrVeh].SelectSingleNode(".//gameName").InnerText;
            makeNameOrig = doc.ChildNodes[1].ChildNodes[2].ChildNodes[indexCurrVeh].SelectSingleNode(".//vehicleMakeName").InnerText;
            audioHashOrig = doc.ChildNodes[1].ChildNodes[2].ChildNodes[indexCurrVeh].SelectSingleNode(".//audioNameHash").InnerText;
            layoutOrig = doc.ChildNodes[1].ChildNodes[2].ChildNodes[indexCurrVeh].SelectSingleNode(".//layout").InnerText;
            typeOrig = doc.ChildNodes[1].ChildNodes[2].ChildNodes[indexCurrVeh].SelectSingleNode(".//type").InnerText;
            classOrig = doc.ChildNodes[1].ChildNodes[2].ChildNodes[indexCurrVeh].SelectSingleNode(".//vehicleClass").InnerText;
            diffuseTintOrig = listVehicles[indexCurrVeh].SelectSingleNode(".//diffuseTint")
                .Attributes["value"].Value;
            string[] vehicleLODs = listVehicles[indexCurrVeh].SelectSingleNode(".//lodDistances")
                .InnerText.Replace(" ", "").Replace("\r\n", ";").Replace("\t", "")
                .TrimStart(';').TrimEnd(';').Split(";");
            vehicleLOD1Orig = vehicleLODs[0];
            vehicleLOD2Orig = vehicleLODs[1];
            vehicleLOD3Orig = vehicleLODs[2];
            vehicleLOD4Orig = vehicleLODs[3];
            vehicleLOD5Orig = vehicleLODs[4];
            vehicleLOD6Orig = vehicleLODs[5];

            modelName.Text = modelNameOrig;
            handlingId.Text = handlingIdOrig;
            gameName.Text = gameNameOrig;
            makeName.Text = makeNameOrig;
            audioHash.Text = audioHashOrig;
            vehLayout.Text = layoutOrig;
            vehType.Text = typeOrig;
            vehClass.Text = classOrig;
            diffuseTint.Text = diffuseTintOrig;
            vehicleLOD1.Text = vehicleLOD1Orig;
            vehicleLOD2.Text = vehicleLOD2Orig;
            vehicleLOD3.Text = vehicleLOD3Orig;
            vehicleLOD4.Text = vehicleLOD4Orig;
            vehicleLOD5.Text = vehicleLOD5Orig;
            vehicleLOD6.Text = vehicleLOD6Orig;

            flagsOrig = listVehicles[indexCurrVeh].SelectSingleNode(".//flags").InnerText.Split(" ");
            for (var indexFlag = 0; indexFlag < ListFlags.Children.Count; indexFlag++)
            {
                var flagGrid = ListFlags.Children[indexFlag] as Grid;
                var flagCheckbox = flagGrid.Children[0] as CheckBox;
                flagCheckbox.IsChecked = false;
            }
            foreach (var flagVeh in flagsOrig)
            {
                for (var indexFlag = 0; indexFlag < ListFlags.Children.Count; indexFlag++)
                {
                    var flagGrid = ListFlags.Children[indexFlag] as Grid;
                    var flagCheckbox = flagGrid.Children[0] as CheckBox;
                    if (flagsList[indexFlag * 2].Replace("__", "_") == flagVeh)
                    {
                        flagCheckbox.IsChecked = true;
                    }
                }
            }

            string tagsFilter = flagsFilter.Text;
            flagsFilter.Text = "";
            flagsFilter.Text = tagsFilter;

            TabItemVehicle.Header = "Véhicule (" + listVehicles[indexCurrVeh].FirstChild.InnerText + ")";

            window.Title = modelNameOrig + " | vehicles.meta Editor";
        }


        private void UpdateXMLData(object sender, RoutedEventArgs e)
        {
            string modelNameVal = modelName.Text;
            string handlingIdVal = handlingId.Text;
            string gameNameVal = gameName.Text;
            XmlElement emptyMakeName = doc.CreateElement("vehicleMakeName");
            string makeNameVal = makeName.Text;
            XmlElement emptyAudio = doc.CreateElement("audioNameHash");
            string audioHashVal = audioHash.Text;
            string layoutVal = vehLayout.Text;
            string typeVal = vehType.Text;
            string classVal = vehClass.Text;
            string diffuseTintVal = diffuseTint.Text;
            string LOD1Val = vehicleLOD1.Text;
            string LOD2Val = vehicleLOD2.Text;
            string LOD3Val = vehicleLOD3.Text;
            string LOD4Val = vehicleLOD4.Text;
            string LOD5Val = vehicleLOD5.Text;
            string LOD6Val = vehicleLOD6.Text;
            string LODsVal = "\r\n        "
                + LOD1Val + "\r\n        "
                + LOD2Val + "\r\n        "
                + LOD3Val + "\r\n        "
                + LOD4Val + "\r\n        "
                + LOD5Val + "\r\n        "
                + LOD6Val + "\r\n      ";
            string flags = "";
            for (var indexFlag = 0; indexFlag < ListFlags.Children.Count; indexFlag++)
            {
                var flagGrid = ListFlags.Children[indexFlag] as Grid;
                var flagCheckbox = flagGrid.Children[0] as CheckBox;
                if (flagCheckbox.IsChecked == true)
                    flags += flagCheckbox.Content.ToString().Replace("__", "_") + " ";
            }
            flags = flags.Trim();

            doc.LoadXml(FilePreview.Text);
            doc.ChildNodes[1].ChildNodes[2].ChildNodes[indexCurrVeh].SelectSingleNode(".//modelName").InnerText = modelNameVal;
            doc.ChildNodes[1].ChildNodes[2].ChildNodes[indexCurrVeh].SelectSingleNode(".//handlingId").InnerText = handlingIdVal;
            doc.ChildNodes[1].ChildNodes[2].ChildNodes[indexCurrVeh].SelectSingleNode(".//gameName").InnerText = gameNameVal;
            if (makeNameVal != "")
                doc.ChildNodes[1].ChildNodes[2].ChildNodes[indexCurrVeh].SelectSingleNode(".//vehicleMakeName").InnerText = makeNameVal;
            else
                doc.ChildNodes[1].ChildNodes[2].ChildNodes[indexCurrVeh].ReplaceChild(emptyMakeName, doc.ChildNodes[1].ChildNodes[2].ChildNodes[indexCurrVeh].SelectSingleNode(".//vehicleMakeName"));
            
            if (audioHashVal != "")
                doc.ChildNodes[1].ChildNodes[2].ChildNodes[indexCurrVeh].SelectSingleNode(".//audioNameHash").InnerText = audioHashVal;
            else
                doc.ChildNodes[1].ChildNodes[2].ChildNodes[indexCurrVeh].ReplaceChild(emptyAudio, doc.ChildNodes[1].ChildNodes[2].ChildNodes[indexCurrVeh].SelectSingleNode(".//audioNameHash"));
            doc.ChildNodes[1].ChildNodes[2].ChildNodes[indexCurrVeh].SelectSingleNode(".//layout").InnerText = layoutVal;
            doc.ChildNodes[1].ChildNodes[2].ChildNodes[indexCurrVeh].SelectSingleNode(".//type").InnerText = typeVal;
            doc.ChildNodes[1].ChildNodes[2].ChildNodes[indexCurrVeh].SelectSingleNode(".//vehicleClass").InnerText = classVal;
            doc.ChildNodes[1].ChildNodes[2].ChildNodes[indexCurrVeh].SelectSingleNode(".//diffuseTint").Attributes["value"].Value = diffuseTintVal;
            doc.ChildNodes[1].ChildNodes[2].ChildNodes[indexCurrVeh].SelectSingleNode(".//lodDistances").InnerText = LODsVal;
            doc.ChildNodes[1].ChildNodes[2].ChildNodes[indexCurrVeh].SelectSingleNode(".//flags").InnerText = flags;


            StringBuilder sb = new();
            TextWriter tr = new StringWriter(sb);
            XmlTextWriter wr = new(tr)
            {
                Formatting = Formatting.Indented
            };
            doc.Save(wr);
            wr.Close();

            FilePreview.Text = sb.ToString();

            LoadFile(sb.ToString());
            docVeh = XDocument.Parse("<Item>" + listVehicles[indexCurrVeh].InnerXml + "</Item>");
            VehiclePreview.Text = docVeh.ToString();
        }

        private void ResetData(object sender, RoutedEventArgs e)
        {
            diffuseTint.Text = diffuseTintOrig;
            vehicleLOD1.Text = vehicleLOD1Orig;
            vehicleLOD2.Text = vehicleLOD2Orig;
            vehicleLOD3.Text = vehicleLOD3Orig;
            vehicleLOD4.Text = vehicleLOD4Orig;
            vehicleLOD5.Text = vehicleLOD5Orig;
            vehicleLOD6.Text = vehicleLOD6Orig;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                originalFileContent = File.ReadAllText(files[0]);
                originalFileName = files[0];

                LoadFile(originalFileContent);

                LoadFileOverlay.Visibility = Visibility.Collapsed;
            }
        }

        private void Window_DragOver(object sender, DragEventArgs e)
        {
            DropOverlay.Visibility = Visibility.Visible;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            DropOverlay.Visibility = Visibility.Collapsed;
        }

        private void FlagsFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox searchBox = sender as TextBox;
            string term = searchBox.Text;

            for (var indexFlag = 0; indexFlag < ListFlags.Children.Count; indexFlag++)
            {
                var flagGrid = ListFlags.Children[indexFlag] as Grid;
                var flagCheckbox = flagGrid.Children[0] as CheckBox;
                if (flagCheckbox.Content.ToString().Contains(term) || 
                    (term.Contains('.') && flagCheckbox.IsChecked == true && flagCheckbox.Content.ToString().Contains(term.Replace(".","")))) 
                    flagGrid.Visibility = Visibility.Visible;
                else
                    flagGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void VehiclePreview_LostFocus(object sender, RoutedEventArgs e)
        {
            string newVehXml = VehiclePreview.Text;
            XmlDocumentFragment docVehNew = doc.CreateDocumentFragment();
            docVehNew.InnerXml = newVehXml;
            doc.ChildNodes[1].ChildNodes[2].ReplaceChild(docVehNew, doc.ChildNodes[1].ChildNodes[2].ChildNodes[indexCurrVeh]);


            StringBuilder sb = new();
            XmlWriterSettings settings = new()
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                doc.Save(writer);
            }
            FilePreview.Text = sb.ToString();
        }

        private void ModelNameValidation(object sender, TextChangedEventArgs e)
        {
            var valTextBox = (sender as TextBox);
            string input = (sender as TextBox).Text;

            if (!Regex.IsMatch(input, @"^([a-z]|[A-Z]|\d)+$"))
                valTextBox.Foreground = new SolidColorBrush(Colors.Red);
            else
                valTextBox.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void HandlingValidation(object sender, TextChangedEventArgs e)
        {
            var valTextBox = (sender as TextBox);
            string input = (sender as TextBox).Text;

            if (!Regex.IsMatch(input, @"^([A-Z]|[a-z]|\d)+$"))
                valTextBox.Foreground = new SolidColorBrush(Colors.Red);
            else
                valTextBox.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void GameNameValidation(object sender, TextChangedEventArgs e)
        {
            var valTextBox = (sender as TextBox);
            string input = (sender as TextBox).Text;

            if (!Regex.IsMatch(input, @"^([A-Z]|\d)+$"))
                valTextBox.Foreground = new SolidColorBrush(Colors.Red);
            else
                valTextBox.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void MakeNameValidation(object sender, TextChangedEventArgs e)
        {
            var valTextBox = (sender as TextBox);
            string input = (sender as TextBox).Text;

            if (!Regex.IsMatch(input, @"^([A-Z]|\d)+$"))
                valTextBox.Foreground = new SolidColorBrush(Colors.Red);
            else
                valTextBox.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void AudioNameValidation(object sender, TextChangedEventArgs e)
        {
            var valTextBox = (sender as TextBox);
            string input = (sender as TextBox).Text;

            if (!Regex.IsMatch(input, @"^([A-Z]|[a-z]|\d)+$"))
                valTextBox.Foreground = new SolidColorBrush(Colors.Red);
            else
                valTextBox.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void LayoutValidation(object sender, TextChangedEventArgs e)
        {
            var valTextBox = (sender as TextBox);
            string input = (sender as TextBox).Text;

            if (!Regex.IsMatch(input, @"^([A-Z]|_|\d)+$"))
                valTextBox.Foreground = new SolidColorBrush(Colors.Red);
            else
                valTextBox.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void LODValidation(object sender, TextChangedEventArgs e)
        {
            var valTextBox = (sender as TextBox);
            string input = (sender as TextBox).Text;

            if (!Regex.IsMatch(input, @"^\d+\.000000$"))
                valTextBox.Foreground = new SolidColorBrush(Colors.Red);
            else
                valTextBox.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void DiffTintValidation(object sender, TextChangedEventArgs e)
        {
            var valTextBox = (sender as TextBox);
            string input = (sender as TextBox).Text;

            if (!Regex.IsMatch(input, @"^(0x([A-F]|[a-f]|\d){8})$"))
                valTextBox.Foreground = new SolidColorBrush(Colors.Red);
            else
                valTextBox.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void LoadFileOverlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileBtn_Click(sender, e);
        }

        private void VehicleSelector_TextInput(object sender, KeyEventArgs e)
        {
            vehicleSelector.IsDropDownOpen = true;
            string term = vehicleSelector.Text;

            for (var indexVeh = 0; indexVeh < vehicleSelector.Items.Count; indexVeh++)
            {
                var vehicleName = vehicleSelector.Items[indexVeh] as ComboBoxItem;
                if (vehicleName.Content.ToString() == term.ToLower())
                {
                    vehicleSelector.SelectedIndex = indexVeh;
                }
                else if (vehicleName.Content.ToString().Contains(term))
                    vehicleName.Visibility = Visibility.Visible;
                else
                    vehicleName.Visibility = Visibility.Collapsed;
            }
        }

        private void FlagsScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            LinearGradientBrush gradientBrushBottom = new()
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1)
            };
            gradientBrushBottom.GradientStops.Add(
                new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.9));
            gradientBrushBottom.GradientStops.Add(
                new GradientStop(Color.FromArgb(70, 0, 0, 0), 1.0));
            
            LinearGradientBrush gradientBrushTop = new()
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1)
            };
            gradientBrushTop.GradientStops.Add(
                new GradientStop(Color.FromArgb(70, 0, 0, 0), 0.0));
            gradientBrushTop.GradientStops.Add(
                new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.1));

            LinearGradientBrush gradientBrushBoth = new()
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1)
            };
            gradientBrushBoth.GradientStops.Add(
                new GradientStop(Color.FromArgb(70, 0, 0, 0), 0.0));
            gradientBrushBoth.GradientStops.Add(
                new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.1));
            gradientBrushBoth.GradientStops.Add(
                new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.9));
            gradientBrushBoth.GradientStops.Add(
                new GradientStop(Color.FromArgb(70, 0, 0, 0), 1.0));



            if (FlagsScroll.VerticalOffset == 0)
            {
                FlagsScroll.Background = gradientBrushBottom;
                Debug.WriteLine("BOTTOM");
            }
            else if (FlagsScroll.VerticalOffset == FlagsScroll.ScrollableHeight)
            {
                FlagsScroll.Background = gradientBrushTop;
                Debug.WriteLine("TOP");
            }
            else
            {
                FlagsScroll.Background = gradientBrushBoth;
                Debug.WriteLine("BOTH");
            }
        }
    }
}
