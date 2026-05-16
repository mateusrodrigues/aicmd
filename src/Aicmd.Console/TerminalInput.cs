using System.Runtime.InteropServices;
using System.Text;

static class TerminalInput
{
    internal static unsafe string ReadLine(FileStream ttyStream, List<string> history)
    {
        var fd = (int)ttyStream.SafeFileHandle.DangerousGetHandle();

        Termios original = default;
        NativeTerminal.tcgetattr(fd, ref original);

        Termios raw = original;
        raw.c_lflag &= ~(NativeTerminal.ICANON | NativeTerminal.ECHO);
        raw.c_cc[NativeTerminal.VTIME] = 0;
        raw.c_cc[NativeTerminal.VMIN] = 1;
        NativeTerminal.tcsetattr(fd, NativeTerminal.TCSANOW, ref raw);

        var buffer = new StringBuilder();
        var historyIndex = history.Count;
        var savedInput = string.Empty;

        try
        {
            while (true)
            {
                int b = ttyStream.ReadByte();

                if (b == -1 || b == '\n' || b == 'ReadLine\r')
                {
                    Console.Error.WriteLine();
                    break;
                }

                if (b == 0x7f || b == 8) // DEL or Backspace
                {
                    if (buffer.Length > 0)
                    {
                        buffer.Remove(buffer.Length - 1, 1);
                        Console.Error.Write("\b \b");
                    }
                    continue;
                }

                if (b == 0x1b) // ESC — start of arrow key sequence
                {
                    int b2 = ttyStream.ReadByte();
                    if (b2 == '[' || b2 == 'O') // CSI (normal) or SS3 (application cursor keys)
                    {
                        int b3 = ttyStream.ReadByte();
                        if (b3 == 'A' && historyIndex > 0) // Up arrow
                        {
                            if (historyIndex == history.Count)
                                savedInput = buffer.ToString();
                            historyIndex--;
                            var entry = history[historyIndex];
                            ClearCurrentInput(buffer.Length);
                            buffer.Clear();
                            buffer.Append(entry);
                            Console.Error.Write(entry);
                        }
                        else if (b3 == 'B' && historyIndex < history.Count) // Down arrow
                        {
                            historyIndex++;
                            var entry = historyIndex == history.Count ? savedInput : history[historyIndex];
                            ClearCurrentInput(buffer.Length);
                            buffer.Clear();
                            buffer.Append(entry);
                            Console.Error.Write(entry);
                        }
                    }
                    continue;
                }

                if (b >= 32) // Printable character
                {
                    buffer.Append((char)b);
                    Console.Error.Write((char)b);
                }
            }
        }
        finally
        {
            NativeTerminal.tcsetattr(fd, NativeTerminal.TCSANOW, ref original);
        }

        return buffer.ToString();
    }

    private static void ClearCurrentInput(int length)
    {
        if (length > 0)
            Console.Error.Write(new string('\b', length) + new string(' ', length) + new string('\b', length));
    }
}

[StructLayout(LayoutKind.Sequential)]
unsafe struct Termios
{
    public uint c_iflag;
    public uint c_oflag;
    public uint c_cflag;
    public uint c_lflag;
    public byte c_line;
    public fixed byte c_cc[32];
    public uint c_ispeed;
    public uint c_ospeed;
}

static class NativeTerminal
{
    internal const uint ICANON = 2;   // 0000002 octal
    internal const uint ECHO = 8;     // 0000010 octal
    internal const int VTIME = 5;
    internal const int VMIN = 6;
    internal const int TCSANOW = 0;

    [DllImport("libc", SetLastError = true)]
    internal static extern int tcgetattr(int fd, ref Termios termios);

    [DllImport("libc", SetLastError = true)]
    internal static extern int tcsetattr(int fd, int optionalActions, ref Termios termios);
}
