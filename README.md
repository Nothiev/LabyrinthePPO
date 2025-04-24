# Maze Navigation PPO

**Auteur·e** : Arnaud THIEVIN  
**Cours** : Programmation par renforcement  
**Date de dépôt** : 24/04/2025

---

## Contexte & Objectif

Ce projet met en œuvre un agent PPO (Proximal Policy Optimization) entraîné pour naviguer dans un labyrinthe généré procéduralement.  
- **Environnement** : grille 2D de taille configurable (width × height)  
- **Agent** : observations positionnelles + état des murs  
- **Objectif** : atteindre la sortie du labyrinthe en un nombre minimal de pas  
- **Métrique principale** : **Success Rate** (taux d’épisodes réussis)

---

## Installation

Bash :
# Clone du dépôt
git clone https://github.com/Nothiev/LabyrinthePPO.git
cd LabyrinthePPO

# Création et activation de l'env. Python
python -m venv .venv
.venv/Scripts/activate

# Install des dépendances
pip install -r requirements.txt


Usage
1. Entraînement
bash
Copier
Modifier
mlagents-learn Python/config/maze_config.yaml --run-id maze_autonomous --force
checkpoint_interval dans le YAML définit la fréquence d’enregistrement.

2. Inférence & Success Rate
En Éditeur Unity (Inference Only)
Chargez le modèle ONNX dans Behavior Parameters → Model.

Passez Behavior Type à Inference Only.

Appuyez sur Play.

Observez la console Unity/VSCode :

mathematica
Copier
Modifier
Episode 1 – Success = 1 – Success rate = 100.00%
Episode 2 – Success = 0 – Success rate = 50.00%
…
Episode 100 – Success rate = ? %

.
├── Assets/                 # Projet Unity (scènes, scripts C#)
│   └── Scripts/
│       ├── MazeAgent.cs    # Agent modifié pour log success rate
│       └── MazeGenerator.cs
├── Builds/
│   └── Maze.exe            # Build standalone
├── Python/
│   ├── config/maze_config.yaml
│   └── evaluate.py
├── docs/
│   ├── presentation.pdf    # Slides de présentation
│   └── gifs/
│       └── navigate.gif    # GIF de démo (agent en action)
├── .gitignore
├── LINK_TO_REPO.txt
├── README.md               # (ce fichier)
└── requirements.txt
