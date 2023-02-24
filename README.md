# AnimationInstancing

As developers, we’re always aware of performance, both in terms of CPU and GPU. Maintaining good performance gets more challenging as scenes get larger and more complex, especially as we add more and more characters. Me and my colleague in Shanghai come across this problem often when helping customers, so we decided to dedicate a few weeks to a project aimed to improve performance when instancing characters. We call the resulting technique Animation Instancing.
> It needs at least Unity5.4.

## Features

* Instancing SkinnedMeshRenderer
* root motion
* attachments
* LOD
* Support mobile platform
* Culling

> Note:
Before running the example, you should select menu Custom Editor -> AssetBundle -> BuildAssetBundle to build asset bundle.

## Attachments

There's a attachment scene. It shows how to use the attachments.
How to setup the object which hold the attachment?

* Open the generator menu -> AnimationInstancing -> Animation Generator
* Enable the attachment checkbox
* Selece the fbx which refrenced by the prefab
* Enable the skeleton's name to generate
* Press the Generate button.

## Webgl?

* As I have tried so far.
  * It works in a PC browser.
  * But not in a mobile browser.

<https://forum.unity.com/threads/drawmeshinstanced-freezes-on-webgl-platform.564853/>

```text
gregume
Joined:Mar 3, 2018

I am using DrawMeshInstanced to render many objects without the overhead of using individual GameObjects. This works great, except on one platform - WebGL.
The first time DrawMeshInstanced is called, there is a severe hitch and it freezes for several seconds.
I assume there is some CPU work being done internally when it goes to render instanced meshes, perhaps sorting and culling. I don't know why this would be an issue for the WebGL platform and no other platform.

This issue is preventing me from using GPU instancing. Does anyone have any insight into this or how to prevent it?

I have attached a simple project with a scene that demonstrates the problem. Thanks for any help!

```

<https://forum.unity.com/threads/batch-drawinstanced-on-webgl-build-in-chrome-occasionally-takes-14-seconds-per-frame.941579/>

```text

We have seen really slow spikes like this on performance before. Investigation led to this ANGLE (== Chrome and Firefox common) issue: https://bugs.chromium.org/p/chromium/issues/detail?id=1072132#c17 .

You can try to debug if that is the case for you by running a Chrome or Firefox profiler over the slow path, and see where it hangs for several seconds. If the hang happens in glLinkProgram, then it strongly suggests that this problem is the cause. If the hang is somewhere else, then it suggests a different root cause.

```
