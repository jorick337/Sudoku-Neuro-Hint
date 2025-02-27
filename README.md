# Sudoku

<p align="center">
   <img src="https://github.com/jorick337/Sudoku/blob/main/MinorFiles/Images(my)/Gameplay.gif">
</p>

<p align="center">
   <img src="https://img.shields.io/badge/Engine-Unity%206000.0.38f1-blueviolet?style=&logo=unity" alt="Engine">
   <img src="https://img.shields.io/badge/Platform-Windows, Linux, MacOs %20-brightgreen?style=&logo=android" alt="Platform">
   <img src="https://img.shields.io/badge/Version-1.0.1-blue" alt="Game Version">
   <img src="https://img.shields.io/badge/Release Date-17.02.2025-red" alt="Release Date">
</p>

## Об игре

**Sudoku** — это классическая игра-головоломка, дополненная искусственным интеллектом для улучшения игрового опыта. 
Проект создан с использованием **PyTorch** для обучения нейронной сети, которая помогает игрокам решать задачи и предлагает подсказки. Модель экспортирована в формат **ONNX** и интегрирована в **Unity** через пакет **Sentis**, что позволяет использовать её непосредственно в игре. Все файлы модели находятся в папке **/MinorFiles/NeuroModel/....**

Игра не только развлекает, но и помогает игрокам учиться, сочетая традиционную логику с современными технологиями.

## О проекте

* **Дата завершения:** Февраль 1, 2025
* **Последнее обновление:** Февраль 17, 2025
* Проект был сделан на **[Unity Engine](https://unity.com/)**
* **Версия движка:** 6000.0.38f1

## Использованные инструменты:

1. **PyTorch** - фреймворк машинного обучения для языка Python с открытым исходным кодом, созданный на базе Torch.
2. **ONNX** - библиотека Python, которая преобразует модель в формат ONNX.
2. **Sentis** - пакет из PacketManager Unity, который использует ONNX обученные нейро модели для их реализации.
