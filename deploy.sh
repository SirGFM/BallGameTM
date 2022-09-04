#!/bin/bash

if [ $# -ne 2 ]; then
    echo "Usage: ${1} version message"
    echo "  version -- The new version (e.g., 1.0);"
    echo "  message -- The tag label (e.g., first version)"
    exit 0
fi

rm -f VERSION.new VERSION.latest

VERSION=v${1}
MESSAGE="${2}"
echo 'Checking the current version...'
git tag -l | grep -qo ${VERSION}
if [ $? -ne 1 ]; then
    echo "Failed to deploy: version/tag has already been deployed"
    exit 1
fi

WS=deploy/${VERSION}
UNITY=/opt/gamedev/unity3d-editors/2020.3.22f1/Editor/Unity
APP=BallGameTM

echo "Updating the game's version..." &&
    sed -i "s/bundleVersion: .*$/bundleVersion: ${VERSION}/g" ProjectSettings/ProjectSettings.asset &&
    git add ProjectSettings/ProjectSettings.asset &&
    git commit -m "Releasing version ${VERSION}" &&
    mkdir -p ${WS}/linux64 &&
    echo 'Building for Linux 64...' &&
    ${UNITY} -quit -batchmode -buildLinux64Player ${WS}/linux64/${APP} &&
    mkdir -p ${WS}/win32 &&
    echo 'Building for Windows 32...' &&
    ${UNITY} -quit -batchmode -buildWindowsPlayer ${WS}/win32/${APP}.exe &&
    mkdir -p ${WS}/win64 &&
    echo 'Building for Windows 64...' &&
    ${UNITY} -quit -batchmode -buildWindows64Player ${WS}/win64/${APP}.exe &&
    echo 'Deploying Linux 64 version...' &&
    echo ${VERSION} > VERSION.new &&
    cp deploy/itch.linux.manifest ${WS}/linux64/.itch.toml &&
    butler push --userversion-file=VERSION.new ${WS}/linux64 "GFM/ballgametm:linux64" &&
    echo 'Deploying Windows 32 version...' &&
    cp deploy/reset-config.bat ${WS}/win32 &&
    cp deploy/itch.win.manifest ${WS}/win32/.itch.toml &&
    butler push --userversion-file=VERSION.new ${WS}/win32 "GFM/ballgametm:win32" &&
    echo 'Deploying Windows 64 version...' &&
    cp deploy/reset-config.bat ${WS}/win64 &&
    cp deploy/itch.win.manifest ${WS}/win64/.itch.toml &&
    butler push --userversion-file=VERSION.new ${WS}/win64 "GFM/ballgametm:win64" &&
    rm -f VERSION.new &&
    echo 'Tagging the version...' &&
    git tag -a -m "${MESSAGE}" ${VERSION} &&
    echo 'Pushing changes to remote repository...' &&
    git push &&
    git push --tags &&
    echo 'DONE!!'
