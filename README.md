# HexFall-Clone

Hexfall Clone is a match-3 game inspired by HexFall.  When 3 hexagonal shaped items are matched by their color player earns score. 

## Build Link
You can find current apk build on my drive.
https://drive.google.com/open?id=14M6WjrkQus5gCFLaBa_JdEwNBgDnSbar

## Grid System

By given grid size, system determines where to create hexagons and put them in a matrix. System is also responsible for :

* Swapping two hexagons via their grid positions
* Finding grid item by its grid position
* Caching world positions of matrix
* Enabling and disabling grid items
* Pooling grid items

## Grid Items

Each grid item has color, neighbor data, possible move count and identity to become a bomb.

## Game Manager

Game Manager holds total score, bomb step and move count. It also handles input events and game logic.

## Input Manager

To select a hexagon tap on screen. To turn selected hexagons swipe right for clockwise turning or swipe left for counter clockwise turning. 

