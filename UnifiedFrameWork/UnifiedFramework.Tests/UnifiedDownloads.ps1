param( [string]$drivername = "IEDriver",[string]$destinationpath = "..\..\UnifiedDownloads")

$srciedriver = "https://github.com/vidyutron/UnifiedFramework/raw/mods/UnifiedDownloadResources/IEDriverServer_Win32_3.0.0/IEDriverServer.exe"
$srcchromedriver = "https://github.com/vidyutron/UnifiedFramework/raw/mods/UnifiedDownloadResources/chromedriver_win32/chromedriver.exe"
$srcedgedriver = "https://github.com/vidyutron/UnifiedFramework/blob/mods/UnifiedDownloadResources/EdgeServer/MicrosoftWebDriver.exe"
$srcmozilladriver = "https://github.com/vidyutron/UnifiedFramework/raw/mods/UnifiedDownloadResources/geckodriver-v0.11.1-win32/geckodriver.exe"
$srcwininspect="https://github.com/vidyutron/UnifiedFramework/raw/mods/UnifiedDownloadResources/WinInspect/Inspect.exe"

$sourcewininspect = Join-Path -Path $destinationpath -ChildPath "WinInspect"
$sourcedriver = ""
$srcdrivername = ""
switch($drivername){
	"IEDriver" {$sourcedriver = $srciedriver;$srcdrivername = "IEDriver";$destinationpath = Join-Path -Path $destinationpath -ChildPath "IEDriver";break}
	"ChromeDriver" {$sourcedriver = $srcchromedriver;$srcdrivername = "ChromeDriver";$destinationpath = Join-Path -Path $destinationpath -ChildPath "ChromeDriver";break}
	"EdgeDriver" {$sourcedriver = $srcedgedriver;$srcdrivername = "EdgeDriver";$destinationpath = Join-Path -Path $destinationpath -ChildPath "EdgeDriver\";break}
	"FirefoxDriver" {$sourcedriver = $srcmozilladriver;$srcdrivername = "FirefoxDriver";$destinationpath = Join-Path -Path $destinationpath -ChildPath "FirefoxDriver\";break}
	"GeckoDriver" {$sourcedriver = $srcmozilladriver;$srcdrivername = "FirefoxDriver";$destinationpath = Join-Path -Path $destinationpath -ChildPath "FirefoxDriver\";break}
	default {"Please provide corrent driver name,please check the case as well!"; break}
}

if(![string]::IsNullOrEmpty($sourcedriver)){
	If (!(Test-Path $destinationpath)) {
   New-Item -Path $destinationpath -ItemType Directory
	}
		If (!(Test-Path $sourcewininspect)) {
   New-Item -Path $sourcewininspect -ItemType Directory
	}
Invoke-WebRequest $srcwininspect -OutFile $sourcewininspect\"WinInspect.exe"
Invoke-WebRequest $sourcedriver -OutFile $destinationpath\$srcdrivername".exe"
}



