param( [string]$drivername = "IEDriver",[string]$destinationpath = "..\SeleniumDriver")

$srciedriver = "https://github.com/vidyutron/UnifiedFramework/raw/mods/UnifiedResources/IEDriverServer_Win32_3.0.0/IEDriverServer.exe"
$srcchromedriver = "https://github.com/vidyutron/UnifiedFramework/raw/mods/UnifiedResources/chromedriver_win32/chromedriver.exe"
$srcedgedriver = "https://github.com/vidyutron/UnifiedFramework/blob/mods/UnifiedResources/EdgeServer/MicrosoftWebDriver.exe"
$srcmozilladriver = "https://github.com/vidyutron/UnifiedFramework/raw/mods/UnifiedResources/geckodriver-v0.11.1-win32/geckodriver.exe"

$sourcedriver=""
$srcdrivername=""
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
Invoke-WebRequest $sourcedriver -OutFile $destinationpath\$srcdrivername".exe"
}



