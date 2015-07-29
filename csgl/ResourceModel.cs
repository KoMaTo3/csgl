using System;
using System.Collections.Generic;
using System.IO;

namespace csgl {

  public class ResourceModel: Resource, IDisposable {

    public Model model {
      get { return this._model; }
      protected set { this._model = value; }
    }

    protected Model _model;

    public ResourceModel()
      : base( "" ) {
    }

    public ResourceModel( string name, string source )
      : base( name ) {
      this.type = ResourceType.MODEL;
      this.model = new Model( name );
      this.model.resource = this;
      this.IsValid = true;

      var linesList = source.Split( '\n' );
      var parser = new TextParser();
      var data = parser.ParseText( source );
      foreach( var pair in data ) {
        switch( pair.Key ) {
          case "mesh":
            {
              this.model.mesh = ( Mesh ) Resource.Get( pair.Value );
            }
          break;
          case "material":
            {
              this.model.material = ( Material ) Resource.Get( pair.Value );
            }
          break;
        }
      }
    }

    ~ResourceModel() {
      this.Dispose( true );
    }

    protected override void Dispose( bool disposing ) {
      if( disposing ) {
        if( this.IsValid ) {
          Console.WriteLine( "~ResourceModel '{0}'", this.name );
          Resource.RemoveResource( this.id );
          this.IsValid = false;
          this.model.Dispose();
          this.model = null;
          Console.WriteLine( "~ResourceModel '{0}' done", this.name );
        }
      }
    }

    public static explicit operator Model( ResourceModel resource ) {
      return resource.model;
    }

    public static explicit operator Mesh( ResourceModel resource ) {
      return resource.model.mesh;
    }

    public static explicit operator Material( ResourceModel resource ) {
      return resource.model.material;
    }
  }
}

