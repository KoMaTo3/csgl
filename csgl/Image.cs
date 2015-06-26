using System;
using Gdk;

namespace csgl {
  public class Image: IDisposable {
    public Resource resource;
    protected Size _size;
    protected byte [] _data;

    public int width {
      get { return this._size.Width; }
      protected set { this._size.Width = value; }
    }

    public int height {
      get { return this._size.Height; }
      protected set { this._size.Height = value; }
    }

    public byte[] data {
      get { return this._data; }
      private set { }
    }

    private Image() {
    }

    public Image( byte [] data, int setWidth, int setHeight ) {
      this._size.Width = setWidth;
      this._size.Height = setHeight;
      int dataLength = setWidth * setHeight * 4;
      this._data = new byte[ dataLength ];
      Buffer.BlockCopy( data, 0, this._data, 0, dataLength );
    }

    ~Image() {
      this.Dispose( true );
    }

    public void Dispose() {
      this.Dispose( true );
      System.GC.SuppressFinalize( this );
    }

    public byte[] GetRawPointer() {
      return this._data;
    }

    protected virtual void Dispose( bool disposing ) {
      if( disposing ) {
        if( this._data != null ) {
          Console.WriteLine( "~Image, bytes[{0}]", this._data.Length );
          this._data = null;
          if( this.resource != null ) {
            this.resource.Dispose();
            this.resource = null;
          }
          Console.WriteLine( "~Image done" );
        }
      }
    }
  }
}

