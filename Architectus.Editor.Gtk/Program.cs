using System;
using Eto.Forms;

namespace Architectus.Editor.Gtk;

public class Program
{
    [STAThread]
    public static void Main()
    {
        new Application(Eto.Platforms.Gtk).Run(new MainForm());
    }
}
