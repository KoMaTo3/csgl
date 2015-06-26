using System;

namespace csgl {
  public class ResourceMesh: Resource, IDisposable {
    public Mesh mesh {
      get { return this._mesh; }
      protected set { this._mesh = value; }
    }

    protected Mesh _mesh;

    private ResourceMesh()
      : base( "" ) {
    }

    public ResourceMesh( string name, string source )
      : base( name ) {
      this.type = ResourceType.MESH;
      this.mesh = new Mesh( name, source );
      this.mesh.resource = this;
      this.IsValid = true;
    }

    ~ResourceMesh() {
      this.Dispose( true );
    }

    protected override void Dispose( bool disposing ) {
      if( disposing ) {
        if( this.IsValid ) {
          Console.WriteLine( "~ResourceMesh '{0}'", this.name );
          Resource.RemoveResource( this.id );
          this.IsValid = false;
          this.mesh.Dispose();
          Console.WriteLine( "~ResourceMesh '{0}' done", this.name );
        }
      }
    }

    public static explicit operator Mesh( ResourceMesh resource ) {
      return resource.mesh;
    }
  }
}

