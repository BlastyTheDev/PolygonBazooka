# Polygon Bazooka (name is subject to change)

## This is the MonoGame port. The original (written with osu!framework) is in the `master` branch. This branch (`monogame`) will soon become the default branch, when the game is sufficiently ported to MonoGame.



I have decided to port the game to MonoGame, then continue development using MonoGame instead of the osu!framework.
I don't think I can continue development after singleplayer with the osu!framework due to a few reasons:
- Limited understanding of the osu!framework
  - I do not know how to use the osu!framework well, and will likely not be able to understand it better due to little documentation. It was meant for the development of osu!lazer, and not for a stacking game.
- MonoGame's larger community
  - There is a much larger community around MonoGame, who help document and guide development using it.
  - MonoGame also has an official documentation, with more examples to help developers like myself who are new to the library
 
---

**Polygon Bazooka** is a stacking game similar to [Puyo Puyo Puzzle Pop](https://www.google.com/search?q=puyo+puyo+puzzle+pop) that I started a while ago in Java, with the intent to have something to use my implementation of the [Glicko-2](http://www.glicko.net/glicko/glicko2.pdf).
Currently, all game assets are created by [Machi](https://x.com/marblechese).

This is a rewrite of the game, using the [osu!framework](https://github.com/ppy/osu-framework) in C#.

## Platforms
Planned support for:
- Windows (probably 10 and above)
- Linux (specifically [Ubuntu](https://ubuntu.com/), as the game is being developed on it)
- Web (by WebAssembly)

MacOS will probably be supported, however, I will not make an effort to compile for MacOS if it is not possible for me.

Mobile support is **not planned**.

## License
**Polygon Bazooka** is licensed under the [GNU General Public License v2.0](https://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html).

[tl;dr](https://www.tldrlegal.com/license/gnu-general-public-license-v2) you are allowed to use this commercially, modify, distribute (including modified versions) as long as you track changes, state the source (this repository), your modified version is on the same license, and you include the original copyright.
