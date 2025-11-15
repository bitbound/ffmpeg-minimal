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
- **Windows 32-bit** - with Desktop Duplication API (DXGI) and GDIGrab
- **Windows 64-bit** - with Desktop Duplication API (DXGI) and GDIGrab
- **macOS Intel** - with AVFoundation for screen capture
- **macOS Apple Silicon (ARM64)** - with AVFoundation for screen capture

## Build Configuration

All builds are configured with:
- **Enabled**: GPL, libx264 encoder, rawvideo decoder, mpegts muxer, pipe protocol, scale/format filters
- **Disabled**: ffplay, ffprobe, documentation, network protocols, most codecs, SDL2, and unused libraries
- **Platform-specific capture**: 
  - Windows: dshow, gdigrab (Desktop Duplication API support via DXGI)
  - Linux: x11grab (X11), lavfi
  - macOS: avfoundation

## Technical Decisions

- **Codec**: H.264 (libx264) - Best browser compatibility via Media Source Extensions
- **Container**: MPEG-TS (mpegts) - Ideal for real-time streaming, widely supported in browsers
- **Screen Capture APIs**:
  - Windows: Desktop Duplication API accessible via gdigrab with `-framerate 30 -f gdigrab -i desktop`
  - Linux: X11grab with `-f x11grab -i :0.0`
  - macOS: AVFoundation with `-f avfoundation -i "Capture screen 0"`

## Usage

Download the appropriate binary from GitHub Actions artifacts and use it to capture your desktop:

### Windows
```bash
ffmpeg -f gdigrab -framerate 30 -i desktop -c:v libx264 -preset ultrafast -tune zerolatency -f mpegts -
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
1. Push to the `main` or `master` branch
2. Create a pull request
3. Manually trigger the workflow from the Actions tab

Artifacts are available for download from the Actions run.