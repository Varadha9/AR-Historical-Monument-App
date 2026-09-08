#!/usr/bin/env bash
set -e

# Auto-detect Android SDK
SDK_ROOT="${ANDROID_HOME:-${ANDROID_SDK_ROOT:-$HOME/Android/Sdk}}"
if [ ! -d "$SDK_ROOT" ]; then
    echo "Error: Android SDK not found at $SDK_ROOT. Please set ANDROID_HOME."
    exit 1
fi

BUILD_TOOLS_DIR=$(find "$SDK_ROOT/build-tools" -mindepth 1 -maxdepth 1 -type d | sort -V | tail -n 1)
PLATFORM_JAR=$(find "$SDK_ROOT/platforms" -name "android.jar" | sort -V | tail -n 1)

echo "=== Android Build Pipeline ==="
echo "SDK Root: $SDK_ROOT"
echo "Build Tools: $BUILD_TOOLS_DIR"
echo "Platform JAR: $PLATFORM_JAR"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

echo "=== 1. Cleaning old build artifacts ==="
rm -f *.apk *.zip classes.dex && rm -rf bin && mkdir -p bin

echo "=== 2. Compiling resources with aapt2 ==="
"$BUILD_TOOLS_DIR/aapt2" compile --dir res -o compiled_res.zip

echo "=== 3. Linking assets, manifest, and resources ==="
"$BUILD_TOOLS_DIR/aapt2" link -I "$PLATFORM_JAR" --manifest AndroidManifest.xml -o base.apk -A assets compiled_res.zip --java src

echo "=== 4. Compiling Java source files ==="
javac -source 8 -target 8 -cp "$PLATFORM_JAR" src/com/exam/armonument/*.java -d bin

echo "=== 5. Dexing bytecode with d8 ==="
"$BUILD_TOOLS_DIR/d8" bin/com/exam/armonument/*.class --output .

echo "=== 6. Packaging classes.dex into APK ==="
zip -u base.apk classes.dex

echo "=== 7. Zipaligning APK ==="
"$BUILD_TOOLS_DIR/zipalign" -v -f 4 base.apk aligned.apk

echo "=== 8. Signing APK ==="
if [ ! -f debug.keystore ]; then
    keytool -genkey -v -keystore debug.keystore -alias androiddebugkey -storepass android -keypass android -keyalg RSA -keysize 2048 -validity 10000 -dname "CN=Android Debug,O=Android,C=US"
fi
"$BUILD_TOOLS_DIR/apksigner" sign --ks debug.keystore --ks-pass pass:android --key-pass pass:android --out AR_Monument.apk aligned.apk

echo "=== 9. Updating release copy ==="
mkdir -p ../releases
cp -f AR_Monument.apk ../releases/AR_Monument.apk

echo "=== Build Complete! AR_Monument.apk is ready ==="
ls -lh AR_Monument.apk
