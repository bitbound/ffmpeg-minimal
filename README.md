## Bitbound FFmpeg

A minimalistic ffmpeg build for Bitbound projects, optimized for desktop screen capture and streaming.

## Features

This repository provides minimal FFmpeg builds with only the essential components for:
- Desktop screen capture (platform-specific APIs)
- H.264 encoding (libx264)
- MPEG-TS container format (ideal for streaming)
- Piping output to stdout

## Supported Platforms

The GitHub Actions workflows automatically build FFmpeg for:
- **Linux AMD64** - with X11grab for screen capture
- **Windows 32-bit** - with choice of GDIGrab or DDAGrab for screen capture
- **Windows 64-bit** - with choice of GDIGrab or DDAGrab for screen capture
- **macOS Intel** - with AVFoundation for screen capture
- **macOS Apple Silicon (ARM64)** - with AVFoundation for screen capture

## Build Configuration

All builds use `--disable-everything` then explicitly enable only what's needed:
- **Enabled**: GPL, libx264 encoder, rawvideo decoder, mpegts muxer, pipe protocol, scale/format filters
- **Platform-specific capture**: 
  - Windows: dshow, and either gdigrab or ddagrab (selected at build time)
  - Linux: x11grab (X11), lavfi
  - macOS: avfoundation

This approach ensures the absolute smallest binary size by disabling all features first, then enabling only the essential components.

## Technical Decisions

- **Codec**: H.264 (libx264) - Best browser compatibility via Media Source Extensions
- **Container**: MPEG-TS (mpegts) - Ideal for real-time streaming, widely supported in browsers
- **Screen Capture APIs**:
  - Windows: GDIGrab (`-f gdigrab -i desktop`) or DDAGrab (`-f ddagrab -i desktop`) for Desktop Duplication API
  - Linux: X11grab with `-f x11grab -i :0.0`
  - macOS: AVFoundation with `-f avfoundation -i "Capture screen 0"`

## Usage

Download the appropriate binary from GitHub Actions artifacts and use it to capture your desktop:

### Windows (GDIGrab)
```bash
ffmpeg -f gdigrab -framerate 30 -i desktop -c:v libx264 -preset ultrafast -tune zerolatency -f mpegts -
```

### Windows (DDAGrab - Desktop Duplication API)
```bash
ffmpeg -f ddagrab -framerate 30 -i desktop -c:v libx264 -preset ultrafast -tune zerolatency -f mpegts -
```

Local testing example:

```
ffmpeg -f ddagrab -framerate 30 -i desktop -vf "format=yuv420p" -c:v libx264 -preset ultrafast -tune zerolatency -f mpegts - > output.ts
```

### Linux (X11)
```bash
ffmpeg -f x11grab -framerate 30 -i :0.0 -c:v libx264 -preset ultrafast -tune zerolatency -f mpegts -
```

### macOS
```bash
ffmpeg -f avfoundation -framerate 30 -i "Capture screen 0" -c:v libx264 -preset ultrafast -tune zerolatency -f mpegts -
```

## Building

The builds are automatically created via GitHub Actions. To trigger a build:
1. Go to the Actions tab in the GitHub repository
2. Select the appropriate workflow:
   - **Build Linux FFmpeg** - for Linux AMD64 builds
   - **Build Windows FFmpeg** - for Windows builds (with gdigrab or ddagrab option)
   - **Build macOS FFmpeg** - for macOS builds
3. Click "Run workflow"
4. For Windows builds, select the screen capture device (gdigrab or ddagrab)
5. Artifacts are available for download from the Actions run

### Windows Build Options

When building for Windows, you can choose between two screen capture methods:
- **gdigrab**: Traditional GDI-based screen capture, compatible with all Windows versions
- **ddagrab**: Desktop Duplication API (DXGI) based capture, requires Windows 8+ but offers better performance