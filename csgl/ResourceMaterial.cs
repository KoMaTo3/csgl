using System;
using System.Collections.Generic;

namespace csgl {

  public class ResourceMaterial: Resource, IDisposable {

    public Material material {
      get { return this._material; }
      protected set { this._material = value; }
    }

    protected Material _material;

    private ResourceMaterial()
      : base( "" ) {
    }

    public ResourceMaterial( string name, string source )
      : base( name ) {
      this.type = ResourceType.MATERIAL;
      this.material = new Material( name );
      this.material.resource = this;
      this.IsValid = true;

      var linesList = source.Split( '\n' );
      var parser = new TextParser();
      var data = parser.ParseText( source );
      foreach( var pair in data ) {
        switch( pair.Key ) {
          case "shader":
            {
              this.material.shader = ( ShaderProgram ) Resource.Get( pair.Value );
            }
          break;
          default: //textures
            {
              Console.WriteLine( "pair.Value = {0}", pair.Value );
              Texture texture = ( Texture ) Resource.Get( pair.Value );
              Console.WriteLine( texture == null ? "null" : "not null" );
              texture.resource.Inc();
              this.material.AddTexture( pair.Key, texture );
              //this.texturesList.Add( pair.Key, texture );
              //this.material.shader.AddUniform( new ShaderProgramUniformTexture( pair.Key, texture ) );
            }
          break;
        }
      }
    }

    ~ResourceMaterial() {
      this.Dispose( true );
    }

    protected override void Dispose( bool disposing ) {
      if( disposing ) {
        if( this.IsValid ) {
          Console.WriteLine( "~ResourceMaterial '{0}'", this.name );
          Resource.RemoveResource( this.id );
          this.IsValid = false;
          this.material.Dispose();
          Console.WriteLine( "~ResourceMaterial '{0}' done", this.name );
        }
      }
    }

    public static explicit operator Material( ResourceMaterial resource ) {
      return resource.material;
    }
  }
}

