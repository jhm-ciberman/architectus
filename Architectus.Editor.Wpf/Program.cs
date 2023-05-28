using System;
using Eto.Forms;

namespace Architectus.Editor.Wpf;

public class Program
{
    [STAThread]
    public static void Main()
    {
        new Application(Eto.Platforms.Wpf).Run(new MainForm());
    }
}
