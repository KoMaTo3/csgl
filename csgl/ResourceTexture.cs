using System;

namespace csgl {
  public class ResourceTexture: Resource, IDisposable {
    public Texture texture {
      get { return this._texture; }
      protected set { this._texture = value; }
    }

    protected Texture _texture;

    private ResourceTexture()
      : base( "" ) {
    }

    public ResourceTexture( string name, string fileName )
      : base( name ) {
      this.type = ResourceType.TEXTURE;
      Image image = ( Image ) ( ResourceImage ) Resource.Get( fileName );
      this.texture = new Texture( name, image );
      this.texture.resource = this;
      this.IsValid = true;
    }

    ~ResourceTexture() {
      this.Dispose( true );
    }

    protected override void Dispose( bool disposing ) {
      if( disposing ) {
        if( this.IsValid ) {
          Console.WriteLine( "~ResourceTexture '{0}'", this.name );
          Resource.RemoveResource( this.id );
          this.IsValid = false;
          this.texture.Dispose();
          Console.WriteLine( "~ResourceTexture '{0}' done", this.name );
        }
      }
    }

    public static explicit operator Texture( ResourceTexture resource ) {
      return resource.texture;
    }
  }
}

