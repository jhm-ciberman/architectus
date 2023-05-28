using System;
using Eto.Forms;

namespace Architectus.Editor.Mac;

public class Program
{
    [STAThread]
    public static void Main()
    {
        new Application(Eto.Platforms.Mac64).Run(new MainForm());
    }
}
