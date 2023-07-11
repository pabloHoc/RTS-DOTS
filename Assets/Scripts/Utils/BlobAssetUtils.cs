using Unity.Collections;
using Unity.Entities;

namespace RTS.Utils
{
    public static class BlobAssetUtils {
        public delegate void ActionRef<TBlobAsset, in TData>(ref BlobBuilder blobBuilder, ref TBlobAsset blobAsset, TData data);
 
        public static BlobAssetReference<TBlobAsset> BuildBlobAsset<TBlobAsset, TData>
            (TData data, ActionRef<TBlobAsset, TData> action) where TBlobAsset : unmanaged {
            
            var blobBuilder = new BlobBuilder(Allocator.Temp);
         
            ref var blobAsset = ref blobBuilder.ConstructRoot<TBlobAsset>();
 
            action.Invoke(ref blobBuilder, ref blobAsset, data);
 
            var blobAssetReference = blobBuilder.CreateBlobAssetReference<TBlobAsset>(Allocator.Persistent);
 
            blobBuilder.Dispose();
 
            return blobAssetReference;
        }
    }
}