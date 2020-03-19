# UnityTK

This is a library of scripts for Unity Engine Projects.
It contains several components almost every game project will need and can be used as a base-framework / toolbox for writing games.

# How to use

This repository can be used as unity package.
Package Manager -> + icon -> Add package from git URL...

# Modules

AssetManagement
----

Asset bundle based asset management system that can be used to implement DLCs, Modding or just to seperate your assets into several asset bundles.
It provides an abstract and easy to use api to query for assets previously loaded and registered.

DataBindings
-----

DataBindings provide a simple and very comfortable way to bind data to arbitrary objects (like UI).
They are employing a tree-like structure that lets you bind to specific field / properties of objects and / or invoke methods on objects inside your tree.

They are most commonly used in some MVVM-like structure.

BehaviourModel
----

Provides several pre-built components that can be used to create abstract behaviour models and employ a modular component based architecture.

Audio
----

The UnityTK audio system provides a very simple and lightweight abstraction layer on top of the unity engine audio system.
It provides the ability to construct game sound systems using events.

Cameras
----

The UnityTK camera system provides an abstract interface to generic camera mode implementations.

BuildSystem
----

Provides very simple interface and pre-built components to automate building unity projects.

Benchmarking
----

Simplistic (micro-)benchmarking system you can use to quickly author benchmarks and execute them in the unity editor.

Serialization
----

Custom serializers implementation that is supposed to be used to (de-)serialize savegames and gamedata.
The XML-Deserializer can serialize and deserialize from and to XML, supporting inheritance.

The prototype system can be used to load game data using a prototype pattern.
It was originally developed as a substitute for ScriptableObjects / GameObjects when working with ECS.

Utility
-----

Provides utility for UnityTK itself and commonly used methods that are handy when working with unity.

