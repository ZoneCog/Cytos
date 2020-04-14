# Cytos
Simulator of morphogenetic systems


Morphogenetic system (M system) is an abstract model motivated by key aspects of morphogenetic phenomena such as self-assembly, growth, homeostasis, self-reproduction and self-healing of evolving systems. Its original purpose is the study of these phenomena, both theoretically and by experimentation, at a computational level abstracted from a biological implementation. Application in biological modeling and research is also worth considering.
Mathematically, M systems rely on basic principles of membrane computing and self-assembly, as well as explicit emphasis on geometrical structures (location and shape) in 3D (or generally, dD) Euclidean space. Theoretical studies have shown that M systems are computational universal, as well as efficient in solving difficult NP problems. Moreover, they have also shown robustness to injuries and self-healing capabilities through extensive computer simulations of specific M systems modeling self-reproduction of a model of a basic eukaryotic cell.
As computer simulations play a crucial tool in study and applications of morphogenetics, we have developed a software package to implement M systems in silico. It consists of two modules, a simulation engine and a visualizing tool based on the Unity game engine. Due to the key role of geometry and self-assembly, we were unable to use known P systems modelling libraries, such as the P-Lingua. 

M systems simulator called Cytos is divided into separated modules which cooperate to produce simulation results. The goal of this motion is to make the dynamics of M systems more likely to reproduce macro-properties observed in actual phenomena being modeled, while preserving their computational feasibility.
A modular architecture has been chosen for simpler development and maintenanc. Cytos consists of two main modules, namely the simulation engine and the visualization module. The simulation engine is built as a standalone Microsoft Windows DLL with a friendly and well described API. 
All modules are covered in a single application with a simple user interface called Cytos. The package affords full functionality needed for a variety of experiments since it is intended to be a universal simulation tool for M systems, as given by their definition. All components (modules) are written in C# and .NET 4.6.1

## Simulation examples
### Boxy hallows - simple box division inspired by "Duplication spell" from Harry Potter
![Boxy hallows](http://mmaverikk.borec.cz/images/Cytos/BoxyHallows.jpg)
### Cytoskeleton - controlled cell division
![Cytoskeleton growth](http://mmaverikk.borec.cz/images/Cytos/Cytoskeleton.jpg)
### Septum - Simulation of growth of prokaryotic cell without nuclei
![Septum](http://mmaverikk.borec.cz/images/Cytos/Septum.jpg)

