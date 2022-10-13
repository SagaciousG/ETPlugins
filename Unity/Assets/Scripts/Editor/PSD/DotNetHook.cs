﻿﻿using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
 
namespace Aspose.Hook.Share
{
    public class DotNetHook : IDisposable
    {
        private bool _disposedValue;
 
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                Remove();
                _disposedValue = true;
            }
        }
 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
 
        ~DotNetHook()
        {
            Dispose(false);
        }
 
        public bool IsEnabled { get; protected set; }
     
        public byte[] FromPtrData { get; private set; }
      
        public MethodBase FromMethod { get; }
   
        public MethodBase ToMethod { get; }
 
        private byte[] _existingPtrData;
        private byte[] _originalPtrData;
 
        private IntPtr _toPtr;
        private IntPtr _fromPtr;
 
        public DotNetHook(MethodBase from, MethodBase to)
        {
            FromMethod = from;
            ToMethod = to;
        }
 
        public void Apply()
        {
            Redirect(FromMethod.MethodHandle, ToMethod.MethodHandle);
            IsEnabled = true;
        }
  
        public void ReApply()
        {
            if (_existingPtrData == null)
            {
                throw new NullReferenceException("ExistingPtrData was null. Call DotNetHook.Remove() to populate the data.");
            }
 
            VirtualProtect(_fromPtr, (IntPtr)5, 0x40, out uint x);
 
            for (var i = 0; i < _existingPtrData.Length; i++)
            {
                Marshal.WriteByte(_fromPtr, i, _existingPtrData[i]);
            }
 
            VirtualProtect(_fromPtr, (IntPtr)5, x, out x);
            IsEnabled = true;
        }
    
        public void Remove()
        {
            if (_originalPtrData == null)
            {
                throw new NullReferenceException("OriginalPtrData was null. Call DotNetHook.Apply() to populate the data.");
            }
 
            VirtualProtect(_fromPtr, (IntPtr)5, 0x40, out uint x);
 
            _existingPtrData = new byte[_originalPtrData.Length];
 
            for (var i = 0; i < _originalPtrData.Length; i++)
            {
                _existingPtrData[i] = Marshal.ReadByte(_fromPtr, i);
                Marshal.WriteByte(_fromPtr, i, _originalPtrData[i]);
            }
 
            VirtualProtect(_fromPtr, (IntPtr)5, x, out x);
            IsEnabled = false;
        }
 
        public T Call<T>(object instance, params object[] args)
        {
            Remove();
            try
            {
                var ret = FromMethod.Invoke(instance, args);
                ReApply();
                return (T)Convert.ChangeType(ret, typeof(T));
            }
            catch (Exception)
            {
                // TODO: On Hook failure
            }
 
            ReApply();
            return default(T);
 
        }
 
        private void Redirect(RuntimeMethodHandle from, RuntimeMethodHandle to)
        {
            RuntimeHelpers.PrepareMethod(from);
            RuntimeHelpers.PrepareMethod(to);
 
            if (_fromPtr == default(IntPtr)) _fromPtr = from.GetFunctionPointer();
            if (_toPtr == default(IntPtr)) _toPtr = to.GetFunctionPointer();
 
            FromPtrData = new byte[32];
            Marshal.Copy(_fromPtr, FromPtrData, 0, 32);
 
            VirtualProtect(_fromPtr, (IntPtr)5, 0x40, out uint x);
 
            if (IntPtr.Size == 8)
            {
                // x64
                _originalPtrData = new byte[13];
 
                // 13
                Marshal.Copy(_fromPtr, _originalPtrData, 0, 13);
 
                Marshal.WriteByte(_fromPtr, 0, 0x49);
                Marshal.WriteByte(_fromPtr, 1, 0xbb);
 
                Marshal.WriteInt64(_fromPtr, 2, _toPtr.ToInt64());
 
                Marshal.WriteByte(_fromPtr, 10, 0x41);
                Marshal.WriteByte(_fromPtr, 11, 0xff);
                Marshal.WriteByte(_fromPtr, 12, 0xe3);
 
            }
            else if (IntPtr.Size == 4)
            {
                // x86
                _originalPtrData = new byte[6];
 
                // 6
                Marshal.Copy(_fromPtr, _originalPtrData, 0, 6);
 
                Marshal.WriteByte(_fromPtr, 0, 0xe9);
                Marshal.WriteInt32(_fromPtr, 1, _toPtr.ToInt32() - _fromPtr.ToInt32() - 5);
                Marshal.WriteByte(_fromPtr, 5, 0xc3);
            }
 
            VirtualProtect(_fromPtr, (IntPtr)5, x, out x);
        }
 
        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtect(IntPtr lpAddress, IntPtr dwSize, uint flNewProtect,out uint lpflOldProtect);
    }
}