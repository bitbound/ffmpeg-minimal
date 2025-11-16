using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
  e.Cancel = true;
  cts.Cancel();
};

var ffmpegPath = "ffmpeg";
var ffmpegArgs = @"-f ddagrab -framerate 30 -i desktop -vf ""format=yuv420p"" -c:v libx264 -f mpegts -";

using var process = new Process
{
  StartInfo = new ProcessStartInfo
  {
    FileName = ffmpegPath,
    Arguments = ffmpegArgs,
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    UseShellExecute = false,
    CreateNoWindow = true
  }
};

process.Start();

// Optional: log stderr in background
_ = Task.Run(() =>
{
  while (!process.StandardError.EndOfStream)
  {
    var line = process.StandardError.ReadLine();
    Console.Error.WriteLine($"[ffmpeg] {line}");
  }
});

// Simulate slow reading from stdout
var buffer = new byte[4096];
var stream = process.StandardOutput.BaseStream;

while (!cts.Token.IsCancellationRequested)
{
  try
  {
    var bytesRead = await stream.ReadAsync(buffer, cts.Token);
    if (bytesRead == 0) break;

    // Simulate slow consumer
    await Task.Delay(100, cts.Token); // 100 ms pause per 4 KB
  }
  catch (OperationCanceledException)
  {
    break;
  }
}


try
{
  await process.WaitForExitAsync(cts.Token);
}
catch (OperationCanceledException) { }

process.Kill(true);

Console.WriteLine("Streaming ended.");