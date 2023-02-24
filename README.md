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

<https://zhuanlan.zhihu.com/p/358719772>

```text
实验结论
根据实验的结果，同时参考官方手册中的内容，可以得出如下结论：

根据实验一的结果，当启用了Auto Graphics API的时候，Unity会尝试支持GLES3.2。但是对比两台手机的表现，可以发现，华为 Nova 3上面的显示是有问题的。目前没有找到出现问题的原因。
根据实验二、三和四的结果，当没有启用Auto Graphics API的时候，可以手动选择、排序Graphics APIs。同时，可以发现，硬件设备支持多个图形API。根据实验结果，可以得出：Unity会根据Graphics APIs在列表中的排序，从上至下，检测硬件设备是否支持相应的图形API。Vulkan支持Graphics.DrawMeshInstancedIndirect()，GLES 2.0不支持Graphics.DrawMeshInstancedIndirect()。而GLES 3可能需要某个版本之后才完全支持Graphics.DrawMeshInstancedIndirect()。
项目中的做法
目前的做法是，选择手动排序Graphics APIs，按照顺序添加了Vulkan、OpenGLES3。但根据官方的统计数据，支持Vulkan的的设备只有53%左右。后面可能需要进行更多的验证，有了新的结果也会更新本文。


```
