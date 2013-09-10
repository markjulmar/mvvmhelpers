using System;
using System.IO;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace JulMar.Windows.IO
{
    /// <summary>
    /// This class takes a .NET Stream and turns it into an IRandomAccessStream.
    /// It is not public, but is instead accessible through the AsRandomAccessStream() extension method.
    /// </summary>
    sealed class RandomAccessStream : IRandomAccessStream
    {
        /// <summary>
        /// The underlying stream.
        /// </summary>
        private readonly Stream _stream;

        /// <summary>
        /// Constructor which takes the stream to layer IRandomAccessStream on top of.
        /// </summary>
        /// <param name="stream"></param>
        public RandomAccessStream(Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Returns an input stream at a specified location in a stream.
        /// </summary>
        /// <returns>
        /// The input stream.
        /// </returns>
        /// <param name="position">The location in the stream at which to begin.</param>
        public IInputStream GetInputStreamAt(ulong position)
        {
            long pos = (long) position;
            if (pos > _stream.Length) 
                throw new IndexOutOfRangeException();
            
            _stream.Position = pos;
            return _stream.AsInputStream();
        }

        /// <summary>
        /// Returns an output stream at a specified location in a stream.
        /// </summary>
        /// <returns>
        /// The output stream.
        /// </returns>
        /// <param name="position">The location in the output stream at which to begin.</param>
        public IOutputStream GetOutputStreamAt(ulong position)
        {
            long pos = (long)position;
            if (pos > _stream.Length) 
                throw new IndexOutOfRangeException();

            _stream.Position = pos;
            return _stream.AsOutputStream();
        }

        /// <summary>
        /// Gets or sets the size of the random access stream.
        /// </summary>
        /// <returns>
        /// The size of the stream.
        /// </returns>
        public ulong Size
        {
            get { return (ulong) _stream.Length; }
            set { _stream.SetLength((long)value); }
        }

        /// <summary>
        /// Gets a value that indicates whether the stream can be read from.
        /// </summary>
        /// <returns>
        /// True if the stream can be read from. Otherwise, false.
        /// </returns>
        public bool CanRead { get { return true; } }

        /// <summary>
        /// Gets a value that indicates whether the stream can be written to.
        /// </summary>
        /// <returns>
        /// True if the stream can be written to. Otherwise, false.
        /// </returns>
        public bool CanWrite { get { return true; } }

        /// <summary>
        /// Gets the byte offset of the stream.
        /// </summary>
        /// <returns>
        /// The number of bytes from the start of the stream.
        /// </returns>
        public ulong Position
        {
            get { return (ulong) _stream.Position; }
        }

        /// <summary>
        /// Sets the position of the stream to the specified value.
        /// </summary>
        /// <param name="position">The new position of the stream.</param>
        public void Seek(ulong position)
        {
            _stream.Seek((long)position, 0);
        }

        /// <summary>
        /// Creates a new instance of a IRandomAccessStream over the same resource as the current stream.
        /// </summary>
        /// <returns>
        /// The new stream. The initial, internal position of the stream is 0.The internal position and lifetime of this new stream are independent from the position and lifetime of the cloned stream.
        /// </returns>
        public IRandomAccessStream CloneStream()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _stream.Dispose();
        }

        /// <summary>
        /// Returns an asynchronous byte reader object.
        /// </summary>
        /// <returns>
        /// The asynchronous operation. 
        /// </returns>
        /// <param name="buffer">The buffer into which the asynchronous read operation places the bytes that are read.</param><param name="count">The number of bytes to read that is less than or equal to the Capacity value.</param><param name="options">Specifies the type of the asynchronous read operation.</param>
        public IAsyncOperationWithProgress<IBuffer, UInt32> ReadAsync(IBuffer buffer, UInt32 count, InputStreamOptions options)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Flushes data asynchronously in a sequential stream.
        /// </summary>
        /// <returns>
        /// The stream flush operation.
        /// </returns>
        public IAsyncOperation<Boolean> FlushAsync()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Writes data asynchronously in a sequential stream.
        /// </summary>
        /// <returns>
        /// The byte writer operation.
        /// </returns>
        /// <param name="buffer">The buffer into which the asynchronous writer operation writes.</param>
        public IAsyncOperationWithProgress<UInt32, UInt32> WriteAsync(IBuffer buffer)
        {
            throw new NotSupportedException();
        }
    }
}
