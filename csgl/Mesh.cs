using System;
using System.Collections.Generic;
using OpenTK;

/*
 * Один Mesh может принадлежать разным объектам, которые имеют разные материалы
*/

namespace csgl {
  public class Mesh: IDisposable, IResourceEntity {
    public Resource resource;
    private Queue< Vector3 > _vertices;
    private Queue< Vector4 > _colors;
    private Queue< UInt32 > _indices;
    private string _name;
    private int _hash;

    public Queue< Vector3 > vertices {
      get { return this._vertices; }
      private set { this._vertices = value; }
    }

    public Queue< Vector4 > colors {
      get { return this._colors; }
      private set { this._colors = value; }
    }

    public Queue< UInt32 > indices {
      get { return this._indices; }
      private set { this._indices = value; }
    }

    public int hash {
      get { return this._hash; }
      private set { this._hash = value; }
    }

    private Mesh() {
    }

    public string name {
      get{ return this._name; }
      private set{ this._name = value; }
    }

    public Mesh( string setName, string source ) {
      this.name = setName;
      this.vertices = new Queue< Vector3 >();
      this.colors = new Queue< Vector4 >();
      this.indices = new Queue< UInt32 >();
      this.hash = this.name.GetHashCode();
      foreach( string line in source.Replace( '.', ',' ).Split( '\n' ) ) {
        if( line.Length < 5 ) { //5 = '0 0 0'
          continue;
        }
        if( line[ 0 ] == 'i' ) { //index
          foreach( string indexStr in line.Substring( 2 ).Split( ' ' ) ) {
            this.indices.Enqueue( UInt32.Parse( indexStr ) );
          }
        } else { //vertice
          string [] floatList = line.Split( ' ' );
          if( floatList.Length < 3 ) {
            Console.WriteLine( "[ERROR] Mesh '{0}' has bad vertice information: '{1}'", this.name, line );
          } else {
            this.vertices.Enqueue( new Vector3( float.Parse( floatList[ 0 ] ), float.Parse( floatList[ 1 ] ), float.Parse( floatList[ 2 ] ) ) );
            if( floatList.Length >= 7 ) { //color
              this.colors.Enqueue( new Vector4( float.Parse( floatList[ 3 ] ), float.Parse( floatList[ 4 ] ), float.Parse( floatList[ 5 ] ), float.Parse( floatList[ 6 ] ) ) );
            } else {
              this.colors.Enqueue( new Vector4( 1.0f, 1.0f, 1.0f, 1.0f ) );
            }
          }
        }
      }
    }

    ~Mesh() {
      this.Dispose( true );
    }

    public void Dispose() {
      this.Dispose( true );
      GC.SuppressFinalize( this );
    }

    public void Inc() {
      if( this.resource != null ) {
        this.resource.Inc();
      }
    }

    public void Dec() {
      if( this.resource != null ) {
        this.resource.Dec();
      }
    }

    protected virtual void Dispose( bool disposing ) {
      if( disposing ) {
        if( this.vertices != null ) {
          Console.WriteLine( "~Mesh {0}", this.name );
          this._vertices = null;
          this._colors = null;
          this._indices = null;
          if( this.resource != null ) {
            this.resource.Dispose();
            this.resource = null;
          }
          Console.WriteLine( "~Mesh {0} done", this.name );
        }
      }
    }
  }
}

