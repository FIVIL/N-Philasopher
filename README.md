# Dining Philosopher Problem
   
![Dining Philosopher Problem](NPhilosopher.JPG?raw=true "Dining Philosopher Problem")

## Description

In computer science, the dining philosophers problem is an example problem often used in concurrent algorithm design to illustrate synchronization issues and techniques for resolving them.

It was originally formulated in 1965 by Edsger Dijkstra as a student exam exercise, presented in terms of computers competing for access to tape drive peripherals. Soon after, Tony Hoare gave the problem its present formulation.[wikipedia](https://en.wikipedia.org/wiki/Dining_philosophers_problem)

## This Project

This is an implementation of an answer to the dining philosopher problem which fully demonstrates the race condition situation. This project uses Peer-to-Peer networking and each instance of an app acts as a complete philosopher. There is also a Thread base and client-server version of this code in the repos.

## Clone

You can simply clone this repo and run the code, using Visual Studio 2017+, after compiling you need to add a text file (config.txt) whit the content below for the app to work correctly, after opening 5 instances of the app, it starts working.

0;1;1010;2020
1;2;2020;3030
2;3;3030;4040
3;4;4040;5050
4;5;5050;1010
