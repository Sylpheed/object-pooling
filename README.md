# Object Pooling

Easy-to-setup, flexible, and performant object pooling system. I've been using this up to this date both for game and enterprise applications.

I wrote this way back in 2017. Since it's been a while, I think I can do better than what I wrote before. I'll be updating this sometime.

`Actual code can be found in Assets/Sylpheed/ObjectPooling`

# How to Use
- Attach `Poolable` to a prefab GameObject or a template GameObject (disabled GameObject in a scene). This is where newly created objects will be based from.
- Create a `PoolSource` object via script to setup pooling. Bind and provide the `Poolable` object during instantiation.
- You can also use `PoolSourceCollection` if you want an editor-friendly MonoBehavior wrapper  on top of `PoolSource`.
- Call `PoolSource.GetObject()` to get an object from the pool. This will automatically instantiate a new object if nothing's in the pool.
- Call `Pool()` or `PoolOrDestroy()` on `GameObject` or a `Component` to pool the object.
- Add `OnPoolAwake()` and/or `OnPoolSleep()` in your MonoBehaviour script in the same object where `Poolable` is attached. This is useful if you want to reset/initialize parameters once an object is taken/returned from the pool.

# Todo
- Convert into a UPM-friendly plugin
- Create an editor-friendly `PoolSource` so that it can be fully setup via editor. This is different from PoolSourceCollection.
- More improvements...
