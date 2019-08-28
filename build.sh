#!/usr/bin/env bash

# kill unity
killall Unity

# go to project root. chmod 777
echo [LiveData] Unity Exporting... {$(pwd)}

# unity path
UNITY_PATH="/Applications/Unity/Hub/Editor/2019.2.2f1/Unity.app/Contents/MacOS/Unity"
$UNITY_PATH -batchmode -nographics -projectPath $(pwd) -executeMethod ExportPackage.ExportPlugin -quit \
    || die "Failed to export package. Make sure the project is not open in Unity"

echo [LiveData] Export Done!
