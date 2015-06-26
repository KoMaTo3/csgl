using System;

namespace csgl {
  public class ResourceImage: Resource, IDisposable {
    protected Image _image;

    public Image image {
      get { return _image; }
      private set { this._image = value; }
    }

    private ResourceImage()
      : base( "" ) {
    }

    public ResourceImage( string name, byte [] srcData )
      : base( name ) {
      this.type = ResourceType.IMAGE;
      this.image = ImageLoader.Load( srcData );
      this.image.resource = this;
    }

    ~ResourceImage() {
      this.Dispose( true );
    }

    protected override void Dispose( bool disposing ) {
      if( disposing ) {
        if( this.image != null ) {
          Console.WriteLine( "~ResourceImage '{0}'", this.name );
          Resource.RemoveResource( this.id );
          this.image.Dispose();
          this.image = null;
          Console.WriteLine( "~ResourceImage '{0}' done", this.name );
        }
      }
    }

    public static explicit operator Image( ResourceImage resource ) {
      return resource.image;
    }
  }
}

