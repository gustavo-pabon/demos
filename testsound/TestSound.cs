using System.Media;

namespace testsound
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    class TestSound
    {
        static void Main(string[] args)
        {
            SoundPlayer player = new SoundPlayer();
            player.SoundLocation = @".\sound.wav";
            player.PlaySync();
        }
    }
}

