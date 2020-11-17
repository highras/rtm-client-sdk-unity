# RTM Client Unity SDK Package Release Docs

# Index

[TOC]

## Release Notice
* If you use the audio features, you need to import voice-related libraries when packaged and released

## Libraries Directory Structure

* **\<rtm-client-sdk-unity\>/Assets/Plugins/iOS**

	IOS libraries. When you release the iOS package, please import this library into the project.
	It includes two files, libaudio-convert.a and libaudio-convert.with-armv7.
	If your project does not require support old device (4 or 4s), please use libaudio-convert.a and remove libaudio-convert.with-armv7
	If your project need to support old device (4 or 4s), please remove the libaudio-convert.a and rename libaudio-convert.with-armv7 into libaudio-convert.a


* **\<rtm-client-sdk-unity\>/Assets/Plugins/Android**

	Android libraries. When you release the Android package, please import this library into the project.

* **\<rtm-client-sdk-unity\>/Assets/Plugins/MacOS**

	MacOS libraries. When you release the MacOS package or use the MacOS editor simulator, please import this library into the project.

* **\<rtm-client-sdk-unity\>/Assets/Plugins/x86**

	Win32 libraries. When you release the Win32 package or use the Win32 editor simulator, please import this library into the project.

* **\<rtm-client-sdk-unity\>/Assets/Plugins/x86_64**

	Win64 libraries. When you release the Win64 package or use the Win64 editor simulator, please import this library into the project.

