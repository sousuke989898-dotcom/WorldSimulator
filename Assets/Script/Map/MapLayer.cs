using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Game.World
{
    public class MapLayer<T>
    {
        public int Width { get; }
        public int Height { get; }
        private readonly T[] _data;

        public MapLayer(int width, int height)
        {
            Width = width;
            Height = height;
            _data = new T[width * height];
        }

        public T[] RawData => _data;
        public Span<T> AsSpan() => _data;

        public ref T this[int x, int y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _data[y * Width + x];
        }

        public ref T this[Vector2Int p]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _data[p.y * Width + p.x];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInBounds(int x, int y) => (uint)x < (uint)Width && (uint)y < (uint)Height;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInBounds(Vector2Int p) => IsInBounds(p.x, p.y);

        public MapLayer<T> Clone()
        {
            var clone = new MapLayer<T>(Width, Height);
            Array.Copy(_data, clone._data, _data.Length);
            return clone;
        }
    }
}