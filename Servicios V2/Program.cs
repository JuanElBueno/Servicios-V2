using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Servicios
{
    class Program
    {
        // Rutas y Variables Globales
        static string rutaBase = @"C:\Juanelbuenocopiadelosarcivos";
        static string optimizacionPath = Path.Combine(rutaBase, "optimizacion");
        static string regPath = Path.Combine(optimizacionPath, "reg");
        static string visualPath = Path.Combine(optimizacionPath, "Microsoft-Visual-C++");
        // URLs DE ACTUALIZACIÓN (CAMBIA ESTO POR TUS LINKS REALES DE GITHUB)
        static string urlVersionTxt = "https://raw.githubusercontent.com/JuanElBueno/Mecha/main/version.txt";
        static string urlNuevoExe = "https://github.com/JuanElBueno/Mecha/raw/main/Servicios.exe";

        // Ruta de WinRAR (necesaria según tu script)
        static string winrarPath = @"C:\Program Files\WinRAR\WinRAR.exe";

        static string versionActual = "1.1";

        // Servicios Generales (Opción 1)
        static string[] listaServicios = {
            "UmRdpService", "edgeupdate", "edgeupdatem", "WdiSystemHost", "WdiServiceHost",
            "DiagTrack", "ShellHWDetection", "WinRM", "tzautoupdate", "ALG", "BTAGService",
            "LanmanServer", "VacSvc", "diagnosticshub.standardcollector.service", "RasAuto",
            "RasMan", "SEMgrSvc", "stisvc", "AarSvc_4fc44", "lmhosts", "iphlpsvc", "PeerDistSvc",
            "Spooler", "SessionEnv", "WpcMonSvc", "DialogBlockingService", "RemoteAccess",
            "LanmanWorkstation", "PrintNotify", "Fax", "MsKeyboardFilter", "GraphicsPerfSvc",
            "ssh-agent", "Wecsvc", "RemoteRegistry", "PcaSvc", "FontCache", "bthserv",
            "SensorDataService", "dmwappushservice", "ScDeviceEnum", "lfsvc", "TabletInputService",
            "SensorService", "SensrSvc", "NetTcpPortSharing", "wisvc", "WerSvc", "PhoneSvc",
            "TermService", "shpamsvc", "SysMain", "SCardSvr", "TapiSrv", "Themes", "RpcLocator",
            "FontCache3.0.0.0", "WSearch", "SCPolicySvc", "autotimesvc", "MixedRealityOpenXRSvc"
        };

        static async Task Main(string[] args)
        {
            Console.Title = $"Juan El Bueno {versionActual} ({(Environment.Is64BitOperatingSystem ? "64 bits" : "32 bits")})";

            if (!EsAdministrador())
            {
                Color("ERROR: Debes ejecutar esta aplicación como Administrador.", ConsoleColor.Red);
                Console.ReadKey();
                return;
            }

            CrearCarpetas();
            await VerificarInternetYActualizaciones();

            while (true)
            {
                MostrarMenu();
            }
        }

        static void MostrarMenu()
        {
            Console.Clear();
            Color("==================================================", ConsoleColor.White);
            Color("=                       MENU                     =", ConsoleColor.White);
            Color("==================================================", ConsoleColor.White);
            Color("* 1) Servicios (Gestionar)                       *", ConsoleColor.White);
            Color("* 2) Características de Windows 10               *", ConsoleColor.White);
            Color("* 3) Reparar Archivos corruptos Windows          *", ConsoleColor.White);
            Color("* 4) Archivos optimizacion Mecha                 *", ConsoleColor.White);
            Color("* 5) Internet CrymiCK + Timer                    *", ConsoleColor.White);
            Color("* 6) Salir                                       *", ConsoleColor.White);
            Color("==================================================", ConsoleColor.White);
            Console.Write("Seleccione una opcion [1-6]: ");

            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1": MenuServicios(); break;
                case "2": DesactivarCaracteristicas(); break;
                case "3": RepararArchivos(); break;
                case "4": OptimizacionMechaCompleta(); break;
                case "5": OptimizarInternet(); break;
                case "6": Environment.Exit(0); break;
                default:
                    Color("OPCION NO VALIDA", ConsoleColor.Yellow);
                    Thread.Sleep(2000);
                    break;
            }
        }

        #region OPCION 4: OPTIMIZACION MECHA COMPLETA
        static void OptimizacionMechaCompleta()
        {
            Console.Clear();
            Color("[+] Poniendo todos los Archivos optimizacion...", ConsoleColor.Yellow);
            Thread.Sleep(2000);

            // 1. Desactivar Servicios Específicos de Mecha
            DesactivarServiciosMecha();

            // 2. Timers
            Color("[+] Configurando Timers (BCDEdit)...", ConsoleColor.Cyan);
            EjecutarComando("bcdedit /set disabledynamictick yes");
            EjecutarComando("bcdedit /deletevalue useplatformclock");
            EjecutarComando("bcdedit /set useplatformtick yes");

            // 3. Limpieza Profunda (Temp, Cookies, Cache)
            LimpiezaProfundaArchivos();

            // 4. Rebuild Counters
            Color("[+] Reconstruyendo contadores de rendimiento...", ConsoleColor.Cyan);
            EjecutarComando("lodctr /r");

            // 5. Windows Update & SoftwareDistribution
            LimpiarWindowsUpdate();

            // 6. PowerShell Mitigations & Memory
            AplicarMitigacionesPowerShell();

            // 7. Registro Masivo
            AplicarRegistroMasivo();

            // 8. Descarga e Instalación de .REG (Reg.rar)
            GestionarRegRar().Wait();

            // 9. Visual C++
            GestionarVisualC().Wait();

            Color("[+] Optimización Mecha Finalizada.", ConsoleColor.Green);
            Console.Write("¿Quieres reiniciar el ordenador ahora? (y/n): ");
            if (Console.ReadLine().ToLower() == "y")
            {
                EjecutarComando("shutdown /r /t 0");
            }
        }

        static void DesactivarServiciosMecha()
        {
            string[] serviciosMecha = {
                "SysMain", "wisvc", "icssvc", "Fax", "SessionEnv", "TermService",
                "bthserv", "TabletInputService", "DiagTrack", "DPS", "DoSvc", "WpnService"
            };

            foreach (var s in serviciosMecha)
            {
                // wmic service where name='X' call ChangeStartmode Disabled
                EjecutarComando($"wmic service where name='{s}' call ChangeStartmode Disabled");
                EjecutarComando($"sc stop \"{s}\"");
            }
        }

        static void LimpiezaProfundaArchivos()
        {
            Color("[+] Iniciando limpieza profunda de archivos temporales...", ConsoleColor.Yellow);

            // Directorio de usuarios
            string usersPath = @"C:\Users";
            if (Directory.Exists(usersPath))
            {
                foreach (var userDir in Directory.GetDirectories(usersPath))
                {
                    string userName = Path.GetFileName(userDir);
                    Console.Title = $"Limpiando usuario: {userName} ...";

                    // Rutas a limpiar dentro de cada usuario
                    string[] rutasLimpiar = {
                        @"cookies",
                        @"Local Settings\Temp",
                        @"AppData\Local\Temp",
                        @"Local Settings\Temporary Internet Files",
                        @"AppData\Local\Microsoft\Windows\Temporary Internet Files",
                        @"AppData\Local\Microsoft\Windows\WER\ReportArchive",
                        @"AppData\Local\Google\Chrome\User Data\Default\Cache",
                        @"AppData\Local\Microsoft\Windows\INetCache", // Edge
                        @"AppData\Local\Microsoft\Windows\INetCookies",
                        @"AppData\Local\Microsoft\Terminal Server Client\Cache", // RDP
                        @"AppData\Local\Opera Software\Opera Next\Cache",
                        @"AppData\Local\Vivaldi\User Data\Default\Cache",
                        @"AppData\Local\BraveSoftware\Brave-Browser\User Data\Default\Cache"
                    };

                    foreach (var subRuta in rutasLimpiar)
                    {
                        string pathCompleta = Path.Combine(userDir, subRuta);
                        BorrarContenidoCarpeta(pathCompleta);
                    }
                }
            }

            // Limpiezas del sistema
            BorrarContenidoCarpeta(Environment.GetEnvironmentVariable("TEMP"));
            BorrarContenidoCarpeta(@"C:\Windows\Temp");
            BorrarContenidoCarpeta(@"C:\Windows\Prefetch");

            // Archivos sueltos específicos
            EjecutarComando("del c:\\WIN386.SWP /f /q");
            EjecutarComando("del *.log /a /s /q /f"); // Cuidado con esto en raíz, pero estaba en el script

            Console.Title = "Archivos eliminados...";
        }

        static void BorrarContenidoCarpeta(string ruta)
        {
            if (!Directory.Exists(ruta)) return;

            try
            {
                // Intentar borrar archivos
                string[] files = Directory.GetFiles(ruta, "*.*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    try { File.Delete(file); } catch { } // Ignorar errores si está en uso
                }

                // Intentar borrar carpetas vacías
                string[] dirs = Directory.GetDirectories(ruta);
                foreach (string dir in dirs)
                {
                    try { Directory.Delete(dir, true); } catch { }
                }
            }
            catch { }
        }

        static void LimpiarWindowsUpdate()
        {
            Color("[+] Limpiando archivos de Windows Update...", ConsoleColor.Cyan);
            EjecutarComando("net stop wuauserv");
            EjecutarComando("net stop UsoSvc");
            EjecutarComando("net stop bits");
            EjecutarComando("net stop dosvc");

            BorrarContenidoCarpeta(@"C:\Windows\SoftwareDistribution");

            // Configuración Regedit Update
            EjecutarComando(@"reg add HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate /v DoNotConnectToWindowsUpdateInternetLocations /t REG_DWORD /d 1 /f");
            EjecutarComando(@"reg add HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate /v SetDisableUXWUAccess /t REG_DWORD /d 1 /f");
            EjecutarComando(@"reg add HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU /v NoAutoUpdate /t REG_DWORD /d 1 /f");
            EjecutarComando(@"reg add HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate /v ExcludeWUDriversInQualityUpdate /t REG_DWORD /d 1 /f");

            EjecutarComando("gpupdate /force");
        }

        static void AplicarMitigacionesPowerShell()
        {
            Color("[+] Aplicando Mitigaciones (PowerShell)...", ConsoleColor.Cyan);
            EjecutarPowerShell("Disable-MMAgent -MemoryCompression");
            EjecutarPowerShell("Clear-RecycleBin -Confirm:$false");

            // Loop complejo de Set-ProcessMitigation del script original
            EjecutarPowerShell("ForEach($v in (Get-Command -Name \\\"Set-ProcessMitigation\\\").Parameters[\\\"Disable\\\"].Attributes.ValidValues){Set-ProcessMitigation -System -Disable $v.ToString() -ErrorAction SilentlyContinue}");

            // Lista explícita
            string mitigations = "DEP, EmulateAtlThunks, SEHOP, ForceRelocateImages, RequireInfo, BottomUp, HighEntropy, StrictHandle, DisableWin32kSystemCalls, AuditSystemCall, DisableExtensionPoints, BlockDynamicCode, AllowThreadsToOptOut, AuditDynamicCode, CFG, SuppressExports, StrictCFG, AuditMicrosoftSigned, AuditStoreSigned, DisableNonSystemFonts, AuditFont, BlockRemoteImageLoads, BlockLowLabelImageLoads, PreferSystem32, AuditRemoteImageLoads, AuditLowLabelImageLoads, AuditPreferSystem32, EnableExportAddressFilter, AuditEnableExportAddressFilter, EnableExportAddressFilterPlus, AuditEnableExportAddressFilterPlus, EnableImportAddressFilter, AuditEnableImportAddressFilter, EnableRopStackPivot, AuditEnableRopStackPivot, EnableRopCallerCheck, AuditEnableRopCallerCheck, EnableRopSimExec, AuditEnableRopSimExec, SEHOP, AuditSEHOP, SEHOPTelemetry, TerminateOnError, DisallowChildProcessCreation, AuditChildProcess";
            EjecutarPowerShell($"set-ProcessMitigation -System -Disable {mitigations}");
        }

        static void AplicarRegistroMasivo()
        {
            // 1. CALCULO DE RAM PARA 'SvcHostSplitThresholdInKB'
            // CORREGIDO: Usamos WMIC directo para no necesitar referencias externas de VisualBasic
            try
            {
                long memVal = 0;
                ProcessStartInfo psi = new ProcessStartInfo("wmic", "os get TotalVisibleMemorySize /format:value");
                psi.RedirectStandardOutput = true;
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;

                using (Process p = Process.Start(psi))
                {
                    string output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();

                    // Analizamos la salida (Ej: TotalVisibleMemorySize=16000000)
                    string[] lines = output.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        if (line.Contains("TotalVisibleMemorySize"))
                        {
                            string valString = line.Split('=')[1].Trim();
                            long.TryParse(valString, out memVal);
                            break;
                        }
                    }
                }

                if (memVal > 0)
                {
                    // Lógica original: set /a ram=%mem% + 1024000
                    long svchostThreshold = memVal + 1024000;
                    EjecutarComando($@"reg add ""HKLM\SYSTEM\CurrentControlSet\Control"" /v ""SvcHostSplitThresholdInKB"" /t REG_DWORD /d ""{svchostThreshold}"" /f");
                }
            }
            catch { } // Si falla el cálculo, continuamos con el resto

            // 2. LISTA COMPLETA DE COMANDOS REGISTRY (Copiados de tu script Batch)
            // Nota: Las comillas dobles (") dentro del comando deben escribirse como dobles ("") en C#
            string[] comandosReg = {
        // --- Windows Update & Seguridad ---
        @"reg add ""HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate"" /v ""DoNotConnectToWindowsUpdateInternetLocations"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate"" /v ""SetDisableUXWUAccess"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU"" /v ""NoAutoUpdate"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate"" /v ""ExcludeWUDriversInQualityUpdate"" /t REG_DWORD /d ""1"" /f",
        
        // --- FileSystem & NTFS (Tu lista masiva) ---
        @"reg add ""HKLM\SYSTEM\ControlSet001\Control\FileSystem"" /v ""ContigFileAllocSize"" /t REG_DWORD /d ""1536"" /f",
        @"reg add ""HKLM\SYSTEM\ControlSet001\Control\FileSystem"" /v ""DisableDeleteNotification"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\ControlSet001\Control\FileSystem"" /v ""DontVerifyRandomDrivers"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SYSTEM\ControlSet001\Control\FileSystem"" /v ""FilenameCache"" /t REG_DWORD /d ""1024"" /f",
        @"reg add ""HKLM\SYSTEM\ControlSet001\Control\FileSystem"" /v ""LongPathsEnabled"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\ControlSet001\Control\FileSystem"" /v ""NtfsAllowExtendedCharacter8dot3Rename"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\ControlSet001\Control\FileSystem"" /v ""NtfsBugcheckOnCorrupt"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\ControlSet001\Control\FileSystem"" /v ""NtfsDisable8dot3NameCreation"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SYSTEM\ControlSet001\Control\FileSystem"" /v ""NtfsDisableCompression"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\ControlSet001\Control\FileSystem"" /v ""NtfsDisableEncryption"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SYSTEM\ControlSet001\Control\FileSystem"" /v ""NtfsEncryptPagingFile"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\ControlSet001\Control\FileSystem"" /v ""NtfsMemoryUsage"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\ControlSet001\Control\FileSystem"" /v ""NtfsMftZoneReservation"" /t REG_DWORD /d ""4"" /f",
        @"reg add ""HKLM\SYSTEM\ControlSet001\Control\FileSystem"" /v ""PathCache"" /t REG_DWORD /d ""128"" /f",
        @"reg add ""HKLM\SYSTEM\ControlSet001\Control\FileSystem"" /v ""RefsDisableLastAccessUpdate"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SYSTEM\ControlSet001\Control\FileSystem"" /v ""UdfsSoftwareDefectManagement"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\ControlSet001\Control\FileSystem"" /v ""Win31FileSystem"" /t REG_DWORD /d ""0"" /f",

        // --- CurrentControlSet FileSystem (Repetido en tu script, lo mantenemos) ---
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\FileSystem"" /v ""ContigFileAllocSize"" /t REG_DWORD /d ""1536"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\FileSystem"" /v ""DisableDeleteNotification"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\FileSystem"" /v ""DontVerifyRandomDrivers"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\FileSystem"" /v ""FilenameCache"" /t REG_DWORD /d ""1024"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\FileSystem"" /v ""LongPathsEnabled"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\FileSystem"" /v ""NtfsAllowExtendedCharacter8dot3Rename"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\FileSystem"" /v ""NtfsBugcheckOnCorrupt"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\FileSystem"" /v ""NtfsDisable8dot3NameCreation"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\FileSystem"" /v ""NtfsDisableCompression"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\FileSystem"" /v ""NtfsDisableEncryption"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\FileSystem"" /v ""NtfsEncryptPagingFile"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\FileSystem"" /v ""NtfsMemoryUsage"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\FileSystem"" /v ""NtfsMftZoneReservation"" /t REG_DWORD /d ""3"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\FileSystem"" /v ""PathCache"" /t REG_DWORD /d ""128"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\FileSystem"" /v ""RefsDisableLastAccessUpdate"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\FileSystem"" /v ""UdfsSoftwareDefectManagement"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\FileSystem"" /v ""Win31FileSystem"" /t REG_DWORD /d ""0"" /f",

        // --- Session Manager & Executive ---
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Executive"" /v ""AdditionalCriticalWorkerThreads"" /t REG_DWORD /d ""22"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Executive"" /v ""AdditionalDelayedWorkerThreads"" /t REG_DWORD /d ""22"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\I/O System"" /v ""CountOperations"" /t REG_DWORD /d ""0"" /f",
        
        // --- Memory Management ---
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management"" /v ""ClearPageFileAtShutdown"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management"" /v ""FeatureSettingsOverride"" /t REG_DWORD /d ""3"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management"" /v ""FeatureSettingsOverrideMask"" /t REG_DWORD /d ""3"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management"" /v ""IoPageLockLimit"" /t REG_DWORD /d ""16710656"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management"" /v ""LargeSystemCache"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management"" /v ""SystemPages"" /t REG_DWORD /d ""4294967295"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management"" /v ""DisablePagingExecutive"" /t REG_DWORD /d ""1"" /f",
        
        // --- Prefetch ---
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters"" /v ""EnableBootTrace"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters"" /v ""EnablePrefetcher"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters"" /v ""EnableSuperfetch"" /t REG_DWORD /d ""0"" /f",

        // --- Multimedia & Network Throttling ---
        @"reg add ""HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile"" /v ""NetworkThrottlingIndex"" /t REG_DWORD /d ""4294967295"" /f",
        @"reg add ""HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile"" /v ""SystemResponsiveness"" /t REG_DWORD /d ""0"" /f",

        // --- LanmanServer (Network Sharing) ---
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters"" /v ""AutoDisconnect"" /t REG_DWORD /d ""4294967295"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters"" /v ""Size"" /t REG_DWORD /d ""3"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters"" /v ""EnableOplocks"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters"" /v ""IRPStackSize"" /t REG_DWORD /d ""32"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters"" /v ""SharingViolationDelay"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters"" /v ""SharingViolationRetries"" /t REG_DWORD /d ""0"" /f",

        // --- Priority Control & Drivers ---
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\PriorityControl"" /v ""ConvertibleSlateMode"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\PriorityControl"" /v ""Win32PrioritySeparation"" /t REG_DWORD /d ""38"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Services\usbxhci\Parameters"" /v ""ThreadPriority"" /t REG_DWORD /d ""31"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Services\USBHUB3\Parameters"" /v ""ThreadPriority"" /t REG_DWORD /d ""31"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Services\nvlddmkm\Parameters"" /v ""ThreadPriority"" /t REG_DWORD /d ""31"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Services\NDIS\Parameters"" /v ""ThreadPriority"" /t REG_DWORD /d ""31"" /f",

        // --- Desktop & User Experience ---
        @"reg add ""HKCU\Control Panel\Desktop"" /v ""AutoEndTasks"" /t REG_SZ /d ""1"" /f",
        @"reg add ""HKCU\Control Panel\Desktop"" /v ""HungAppTimeout"" /t REG_SZ /d ""1000"" /f",
        @"reg add ""HKCU\Control Panel\Desktop"" /v ""WaitToKillAppTimeout"" /t REG_SZ /d ""2000"" /f",
        @"reg add ""HKCU\Control Panel\Desktop"" /v ""LowLevelHooksTimeout"" /t REG_SZ /d ""1000"" /f",
        @"reg add ""HKCU\Control Panel\Desktop"" /v ""MenuShowDelay"" /t REG_SZ /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control"" /v ""WaitToKillServiceTimeout"" /t REG_SZ /d ""2000"" /f",
        @"reg add ""HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Schedule\Maintenance"" /v ""MaintenanceDisabled"" /t REG_DWORD /d ""1"" /f",
        
        // --- Power Settings & Throttling ---
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Power"" /v ""HibernateEnabled"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Power\PowerThrottling"" /v ""PowerThrottlingOff"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Services\GpuEnergyDrv"" /v ""Start"" /t REG_DWORD /d ""4"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\GraphicsDrivers\Scheduler"" /v ""EnablePreemption"" /t REG_DWORD /d ""0"" /f",
        
        // --- Apps & Game DVR ---
        @"reg add ""HKLM\SOFTWARE\Microsoft\PolicyManager\default\ApplicationManagement\AllowGameDVR"" /v ""value"" /t REG_SZ /d ""00000000"" /f",
        @"reg add ""HKLM\SOFTWARE\Microsoft\PolicyManager\default\ApplicationManagement\AllowSharedUserAppData"" /v ""value"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKCU\Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications"" /v ""GlobalUserDisabled"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKCU\Software\Microsoft\Windows\CurrentVersion\Search"" /v ""BackgroundAppGlobalToggle"" /t REG_DWORD /d ""0"" /f",

        // --- TCPIP Priority (Network) ---
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces"" /v ""DisableTaskOffload"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\ServiceProvider"" /v ""LocalPriority"" /t REG_DWORD /d ""4"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\ServiceProvider"" /v ""HostsPriority"" /t REG_DWORD /d ""5"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\ServiceProvider"" /v ""DnsPriority"" /t REG_DWORD /d ""6"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\ServiceProvider"" /v ""NetbtPriority"" /t REG_DWORD /d ""7"" /f",
        
        // --- Servicios START (Deshabilitar/Optimizar) ---
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Services\Spooler"" /v ""Start"" /t REG_DWORD /d ""4"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Services\PrintNotify"" /v ""Start"" /t REG_DWORD /d ""4"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Services\MapsBroker"" /v ""Start"" /t REG_DWORD /d ""4"" /f",

        // --- Latencias y Power avanzado (Lista final larga del script) ---
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Power"" /v ""ExitLatency"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Power"" /v ""ExitLatencyCheckEnabled"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Power"" /v ""Latency"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Power"" /v ""LatencyToleranceDefault"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Power"" /v ""LatencyToleranceFSVP"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Power"" /v ""LatencyTolerancePerfOverride"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Power"" /v ""LatencyToleranceScreenOffIR"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Power"" /v ""LatencyToleranceVSyncEnabled"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Power"" /v ""RtlCapabilityCheckLatency"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Power"" /v ""QosManagesIdleProcessors"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Power"" /v ""DisableVsyncLatencyUpdate"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Power"" /v ""DisableSensorWatchdog"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Power"" /v ""CoalescingTimerInterval"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Power"" /v ""InterruptSteeringDisabled"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Power"" /v ""LowLatencyScalingPercentage"" /t REG_DWORD /d ""100"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Power"" /v ""HighPerformance"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Power"" /v ""HighestPerformance"" /t REG_DWORD /d ""1"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Power"" /v ""MinimumThrottlePercent"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Power"" /v ""MaximumThrottlePercent"" /t REG_DWORD /d ""0"" /f",
        @"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Power"" /v ""MaximumPerformancePercent"" /t REG_DWORD /d ""100"" /f",

        // --- Fortnite Priority Class ---
        @"reg add ""HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\FortniteClient-Win64-Shipping.exe\PerfOptions"" /v ""CpuPriorityClass"" /t REG_DWORD /d ""3"" /f"
    };

            // 3. Ejecutar el bucle
            foreach (var cmd in comandosReg)
            {
                EjecutarComando(cmd);
            }

            // 4. Comandos TCP Heuristics
            EjecutarComando("netsh interface tcp set heuristics disabled");

            // 5. USB IDLING (Es un bucle complejo en batch, aquí se hace dinámico)
            // El script original hace un FOR sobre Win32_USBHub. En C# lo hacemos así:
            try
            {
                // Esto equivale al loop FOR /F %%a in ('WMIC PATH Win32_USBHub...') del batch
                // Pero para no complicar el código con WMI ManagementObjectSearcher (que requiere añadir Referencias),
                // Ejecutamos el comando REG ADD global de HighPerformance que cubre la mayoría de casos. 
                // Lo más fiel al script Batch original es invocar a CMD para que haga ese trabajo sucio:
                string comandoBatchUSB = "FOR /F \"tokens=*\" %a in ('WMIC PATH Win32_USBHub GET DeviceID^| FINDSTR /L \"VID_\"') DO (REG ADD \"HKLM\\SYSTEM\\CurrentControlSet\\Enum\\%a\\Device Parameters\" /F /V \"EnhancedPowerManagementEnabled\" /T REG_DWORD /d 0)";
                EjecutarComando(comandoBatchUSB);
            }
            catch { }

            Color("[+] Registro masivo aplicado.", ConsoleColor.Green);
        }

        static async Task GestionarRegRar()
        {
            string urlReg = "https://github.com/JuanElBueno/Mecha/raw/refs/heads/main/reg.rar";
            string fileReg = Path.Combine(optimizacionPath, "reg.rar");

            if (!File.Exists(winrarPath))
            {
                Color("ERROR: No tienes WinRAR instalado en la ruta por defecto.", ConsoleColor.Red);
                return;
            }

            if (!File.Exists(fileReg))
            {
                Color("[+] Descargando reg.rar...", ConsoleColor.Yellow);
                await DescargarArchivo(urlReg, fileReg);
            }

            if (File.Exists(fileReg))
            {
                Color("[+] Extrayendo reg.rar...", ConsoleColor.Green);
                // Usamos WinRAR por línea de comandos como en el script original
                EjecutarComando($"\"{winrarPath}\" x -o+ \"{fileReg}\" \"{regPath}\"");

                Thread.Sleep(2000);

                // Aplicar todos los .reg excepto el bluetooth
                if (Directory.Exists(regPath))
                {
                    string[] regFiles = Directory.GetFiles(regPath, "*.reg");
                    foreach (var reg in regFiles)
                    {
                        if (Path.GetFileName(reg) != "OPTIONAL Disable Bluetooth Services.reg")
                        {
                            Console.WriteLine($"Aplicando registro: {Path.GetFileName(reg)}");
                            EjecutarComando($"regedit /s \"{reg}\"");
                        }
                    }

                    Console.Write("¿Si quieres activar bluetooth? (y/n): ");
                    if (Console.ReadLine().ToLower() == "y")
                    {
                        string btReg = Path.Combine(regPath, "OPTIONAL Disable Bluetooth Services.reg");
                        if (File.Exists(btReg)) EjecutarComando($"regedit /s \"{btReg}\"");
                    }
                }
            }
        }

        static async Task GestionarVisualC()
        {
            string urlVisual = "https://github.com/JuanElBueno/Mecha/releases/download/1.70/Microsoft-Visual-C++.rar";
            string fileVisual = Path.Combine(optimizacionPath, "Microsoft-Visual-C++.rar");

            if (!File.Exists(winrarPath)) return;

            if (!File.Exists(fileVisual))
            {
                Color("[+] Descargando Microsoft-Visual-C++.rar...", ConsoleColor.Yellow);
                await DescargarArchivo(urlVisual, fileVisual);
            }

            if (File.Exists(fileVisual))
            {
                Color("[+] Instalando Visual C++ Runtimes...", ConsoleColor.Green);
                EjecutarComando($"\"{winrarPath}\" x -o+ \"{fileVisual}\" \"{visualPath}\"");
                Thread.Sleep(2000);

                if (Directory.Exists(visualPath))
                {
                    // Ejecutar instaladores
                    string[] installers = Directory.GetFiles(visualPath, "*.exe");
                    // O ejecutar el install_all.bat si existe, o uno por uno
                    // En el script original se llaman uno por uno con /passive /norestart

                    // Lógica simplificada: Buscar instaladores y ejecutarlos en silencio
                    // NOTA: Para replicar exactamente el script, deberíamos ejecutar "vcredist2005_x86.exe /q", etc.
                    // Pero como C# lista archivos, podemos iterar.

                    // Ejecutar instalador AIO si existe (mencionado al final del script)
                    string aio = Path.Combine(visualPath, "VisualCppRedist_AIO_x86_x64.exe");
                    if (File.Exists(aio))
                    {
                        EjecutarComando($"\"{aio}\" /Y");
                    }
                    else
                    {
                        // Fallback manual
                        Color("Ejecutando instaladores individuales (esto puede tardar)...", ConsoleColor.Gray);
                        string argsInstall = "/passive /norestart";
                        foreach (var exe in installers)
                        {
                            EjecutarComando($"\"{exe}\" {argsInstall}");
                        }
                    }
                }
            }
        }
        #endregion

        #region MÉTODOS AUXILIARES
        static async Task DescargarArchivo(string url, string destino)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        using (var fs = new FileStream(destino, FileMode.CreateNew))
                        {
                            await response.Content.CopyToAsync(fs);
                        }
                        Color("Descarga completada.", ConsoleColor.Green);
                    }
                    else
                    {
                        Color("Error en descarga.", ConsoleColor.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                Color($"Error descargando: {ex.Message}", ConsoleColor.Red);
            }
        }

        #region LOGICA ANTERIOR (SERVICIOS, ETC)
        static void MenuServicios()
        {
            Console.Clear();
            Color("================ SERVICIOS ================", ConsoleColor.White);
            Color("* 1) Servicios OFF (Desactivar todo)      *", ConsoleColor.White);
            Color("* 2) Servicios ON (Activar todo)          *", ConsoleColor.White);
            Color("===========================================", ConsoleColor.White);
            Console.Write("Seleccione: ");

            string opcion = Console.ReadLine();
            bool activar = (opcion == "2");

            if (opcion != "1" && opcion != "2") return;

            // Pregunta Xbox
            Console.Write("¿Quieres activar los servicios de Xbox? (y/n): ");
            bool xboxOn = (Console.ReadLine().ToLower() == "y");
            GestionarXbox(xboxOn);

            Console.WriteLine();
            Color($"[+] {(activar ? "Iniciando" : "Deteniendo")} lista de servicios...", ConsoleColor.Yellow);
            Console.WriteLine("------------------------------------------------");

            foreach (var servicio in listaServicios)
            {
                if (activar)
                {
                    // VISUALIZACIÓN: Escribe en verde qué servicio se activa
                    Console.Write("[ON] Activando: ");
                    Color(servicio, ConsoleColor.Green);

                    EjecutarComando($"net start {servicio}");
                    EjecutarComando($"sc config {servicio} start= auto");
                }
                else
                {
                    // VISUALIZACIÓN: Escribe en rojo qué servicio se desactiva
                    Console.Write("[OFF] Desactivando: ");
                    Color(servicio, ConsoleColor.Red);

                    EjecutarComando($"net stop {servicio} /y");
                    EjecutarComando($"sc config {servicio} start= disabled");
                }
            }
            ;
            Color("[+] Proceso de servicios finalizado.", ConsoleColor.Green);
            Thread.Sleep(3000);
        }

        static void GestionarXbox(bool activar)
        {
            string[] xboxServices = { "XblGameSave", "XboxNetApiSvc", "XboxGipSvc", "XblAuthManager" };

            Console.WriteLine();
            Color($"--- {(activar ? "Activando" : "Desactivando")} Xbox Services ---", ConsoleColor.Cyan);

            foreach (var svc in xboxServices)
            {
                if (activar)
                {
                    Console.WriteLine($"Xbox [ON]: {svc}");
                    EjecutarComando($"sc config {svc} start= auto");
                    EjecutarComando($"net start {svc}");
                }
                else
                {
                    Console.WriteLine($"Xbox [OFF]: {svc}");
                    EjecutarComando($"net stop {svc} /y");
                    EjecutarComando($"sc config {svc} start= disabled");
                }
            }
        }

        static void DesactivarCaracteristicas()
        {
            string[] features = {
                "Internet-Explorer-Optional-amd64", "Printing-Foundation-InternetPrinting-Client",
                "Printing-Foundation-Features", "WorkFolders-Client", "MSRDC-Infrastructure",
                "SearchEngine-Client-Package", "Printing-XPSServices-Features", "Printing-PrintToPDFServices-Features",
                "SmbDirect", "WindowsMediaPlayer", "MediaPlayback", "WCF-TCP-PortSharing45",
                "WCF-Services45", "DirectPlay", "LegacyComponents"
            };
            Color("[+] Deshabilitando características...", ConsoleColor.Yellow);
            foreach (var f in features) EjecutarPowerShell($"Disable-WindowsOptionalFeature -FeatureName \"{f}\" -Online -NoRestart");
            Console.ReadKey();
        }

        static void RepararArchivos()
        {
            EjecutarComando("DISM /Online /Cleanup-Image /CheckHealth");
            EjecutarComando("DISM /Online /Cleanup-Image /ScanHealth");
            EjecutarComando("DISM /Online /Cleanup-Image /RestoreHealth");
            EjecutarComando("echo y | chkdsk C: /F /R");
            EjecutarComando("DISM /Online /Cleanup-Image /RestoreHealth /Source:C:/RepairSource/Windows /LimitAccess");
        }

        static void OptimizarInternet()
        {
            EjecutarComando("bcdedit /set useplatformtick yes");
            EjecutarComando("ipconfig /flushdns");
            EjecutarComando("netsh int ip reset");
            EjecutarComando("netsh int ipv4 reset");
            EjecutarComando("netsh int ipv6 reset");
            EjecutarComando("netsh winsock reset");
        }

        static void CrearCarpetas()
        {
            string[] carpetas = { rutaBase, optimizacionPath, regPath, visualPath };
            foreach (var c in carpetas) if (!Directory.Exists(c)) Directory.CreateDirectory(c);
        }
        static async Task VerificarInternetYActualizaciones()
        {
            Color("Comprobando conectividad y actualizaciones...", ConsoleColor.Yellow);
            try
            {
                using (var ping = new Ping())
                {
                    var reply = ping.Send("8.8.8.8");
                    if (reply.Status == IPStatus.Success)
                    {
                        Color("[+] Conexión establecida.", ConsoleColor.Green);
                        // Llamamos al sistema de update
                        await SistemaAutoUpdate();
                    }
                    else
                    {
                        throw new Exception("Ping fallido");
                    }
                }
            }
            catch
            {
                Color("No tienes internet o no se pudo conectar. Se omite la actualización.", ConsoleColor.Red);
                Thread.Sleep(2000);
            }
        }

        static async Task SistemaAutoUpdate()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // 1. Obtener la versión del servidor (GitHub)
                    // Configura un tiempo de espera corto por si GitHub va lento
                    client.Timeout = TimeSpan.FromSeconds(5);
                    string versionServerStr = await client.GetStringAsync(urlVersionTxt);
                    versionServerStr = versionServerStr.Trim(); // Quitar espacios o saltos de linea

                    // Convertir a objetos Version para comparar correctamente (así 1.10 es mayor que 1.9)
                    Version versionLocal = Version.Parse(versionActual);
                    Version versionRemota = Version.Parse(versionServerStr);

                    if (versionRemota > versionLocal)
                    {
                        Console.Clear();
                        Color("==================================================", ConsoleColor.Yellow);
                        Color("           ACTUALIZACION ENCONTRADA               ", ConsoleColor.Yellow);
                        Color("==================================================", ConsoleColor.Yellow);
                        Console.WriteLine();
                        Color($"Mi version:    {versionActual}", ConsoleColor.Gray);
                        Color($"Nueva version: {versionServerStr}", ConsoleColor.Green);
                        Console.WriteLine();
                        Console.WriteLine("[Y] SI, Quiero Actualizar");
                        Console.WriteLine("[N] No, ahora no");
                        Console.Write("Seleccione opcion: ");

                        string respuesta = Console.ReadLine().ToLower();

                        if (respuesta == "y")
                        {
                            Color("[+] Descargando nueva versión...", ConsoleColor.Cyan);

                            // Nombre del ejecutable actual y el temporal
                            string exeActual = Process.GetCurrentProcess().MainModule.FileName;
                            string exeNuevo = Path.Combine(Path.GetDirectoryName(exeActual), "Update_Temp.exe");

                            // 2. Descargar el nuevo EXE
                            var exeBytes = await client.GetByteArrayAsync(urlNuevoExe);
                            File.WriteAllBytes(exeNuevo, exeBytes);

                            Color("[+] Descarga finalizada. Reiniciando para aplicar...", ConsoleColor.Green);
                            Thread.Sleep(2000);

                            // 3. Crear el BAT "Mágico" que hace el cambiazo
                            // Explicación:
                            // timeout 2: Espera a que la app C# se cierre del todo.
                            // del: Borra el exe viejo.
                            // ren: Renombra el descargado al nombre original.
                            // start: Arranca la nueva versión.
                            // del: Se borra a sí mismo (el bat).
                            string nombreExeSolo = Path.GetFileName(exeActual);
                            string batScript = $@"
                            @echo off
                            timeout /t 2 /nobreak >nul
                            del ""{exeActual}""
                            ren ""{exeNuevo}"" ""{nombreExeSolo}""
                            start ""JuanElBueno"" ""{exeActual}""
                            del ""%~f0""
                            ";
                            string batPath = Path.Combine(Path.GetDirectoryName(exeActual), "updater.bat");
                            File.WriteAllText(batPath, batScript);

                            // 4. Ejecutar el BAT y cerrar esta app inmediatamente
                            ProcessStartInfo psi = new ProcessStartInfo(batPath)
                            {
                                CreateNoWindow = true,
                                UseShellExecute = true
                            };
                            Process.Start(psi);
                            Environment.Exit(0);
                        }
                    }
                    else
                    {
                        Color($"[+] Tienes la última versión ({versionActual}).", ConsoleColor.Green);
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception ex)
            {
                Color($"Error buscando actualizaciones: {ex.Message}", ConsoleColor.Red);
                Thread.Sleep(2000); // Dar tiempo a leer el error
            }
        }

        static void EjecutarComando(string comando) => ProcessProcessStartInfo(comando, "cmd.exe");
        static void EjecutarPowerShell(string comando) => ProcessProcessStartInfo(comando, "powershell.exe");

        static void ProcessProcessStartInfo(string comando, string fileName)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(fileName, fileName == "cmd.exe" ? "/c " + comando : comando)
                {
                    RedirectStandardOutput = false, // Mostrar output si quieres ver progreso real
                    RedirectStandardError = false,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas"
                };
                using (Process p = Process.Start(psi)) { p.WaitForExit(); }
            }
            catch { }
        }

        static void Color(string texto, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(texto);
            Console.ResetColor();
        }

        static bool EsAdministrador()
        {
            using (WindowsIdentity id = WindowsIdentity.GetCurrent())
                return new WindowsPrincipal(id).IsInRole(WindowsBuiltInRole.Administrator);
        }
        #endregion
        #endregion
    }
}