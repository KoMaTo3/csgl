using System;
using Gtk;

namespace csgl {
  class MainClass {
    public static void Main( string [] args ) {
      using( Core core = new Core() ) {
        core.Run();
      }
    }
  }
}
