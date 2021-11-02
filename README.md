# CDRec_benchmark

## Overview 


- **Paper**: Mourad Khayati, Philippe Cudré-Mauroux, and Michael H. Böhlen: [Scalable Recovery of Missing Blocks in Time Series with High and Low Cross-Correlations](https://rdcu.be/b32bv). KAIS 2020.
- **Algorithms**: The benchmark evaluates all the algorithms mentioned in the paper: CDRec, GROUSE, MRNN, SSA, STMVL, TKCM and TRMF. To enable/disable any algorithm, please refer to the *Algorithms customization* section below.
- **Datasets**: The benchmark evaluates all the datasets used in the paper: gas (drfit6), gas (drfit10), baseball, meteo, tempretaure, and BAFU. To enable/disable any dataset, please refer to the *Datasets customization* section below.
- **Scenarios**: The benchmark will execute the full set of 11 recovery scenarios and report the error using RMSE, MSE and MAE. 
A detailed description of the recovery scenarios can be found [here](https://github.com/eXascaleInfolab/bench-vldb20/blob/master/TestingFramework/README.md).

[**Prerequisites**](#prerequisites) | [**Build**](#build) | [**Execution**](#execution) | [**Citation**](#citation)


___
## Prerequisites 
- Ubuntu 16 or Ubuntu 18 (including Ubuntu derivatives, e.g., Xubuntu) or the same distribution under WSL.
- Clone this repository.
- Mono: Install mono from https://www.mono-project.com/download/stable/ (takes few minutes).

___
## Build

- Build the Testing Framework using the installation script located in the root folder (takes several minutes):
```bash
    $ sh install_linux.sh
```
___

## Execution


```bash
    $ cd TestingFramework/bin/Debug/
    $ mono TestingFramework.exe
```


- **Results**: All results will be added to `Results` folder. The accuracy results of all algorithms will be sequentially added for each scenario and dataset to: `Results/.../.../.../error/`. The runtime results of all algorithms will be added to: `Results/.../.../.../runtime/`. The plots of the recovered blocks will be added to the folder `Results/.../.../.../plots/`.

- **Warning**: The test suite with the default setup will take ~20 hours to finish  and will produce up to 3GB of output files with all recovered data and plots unless stopped early.


___

### Parametrized execution

### Customize datasets

To add a dataset to the benchmark
- import the file to `TestingFramework/bin/Debug/data/{name}/{name}_normal.txt`
- - Requirements: >= 10 columns, >= 1'000 rows, column separator - empty space, row separator - newline
- add `{name}` to the list of datasets in `TestingFramework/config.cfg`

#### Customize algorithms

To exclude an algorithm from the benchmark
- open the file `TestingFramework/config.cfg`
- add an entry `IgnoreAlgorithms =` and specify the list of algorithm codes to exclude them
- the line starting with `#IgnoreAlgorithms =` provides codes for all the algorithms in the benchmark


___
## Citation
```bibtex
@article{DBLP:journals/kais/KhayatiCB20,
  author    = {Mourad Khayati and
               Philippe Cudr{\'{e}}{-}Mauroux and
               Michael H. B{\"{o}}hlen},
  title     = {Scalable recovery of missing blocks in time series with high and low
               cross-correlations},
  journal   = {Knowl. Inf. Syst.},
  volume    = {62},
  number    = {6},
  pages     = {2257--2280},
  year      = {2020},
  url       = {https://doi.org/10.1007/s10115-019-01421-7},
  doi       = {10.1007/s10115-019-01421-7},
  biburl    = {https://dblp.org/rec/journals/kais/KhayatiCB20.bib},
}
```

<!---
___

## Installation (macOS) -- Experimental

### Prerequisites and dependencies 

- The benchmark runs on macOS with a few caveats:
- - TRMF algorithm is disabled (it doesn't work under octave on macOS).
- - The installation takes longer than Linux.
- macOS 10.13 or higher, homebrew
- Sudo rights on the user
- Clone the repository
```bash
    $ xcode-select --install
    $ git clone https://github.com/eXascaleInfolab/bench-vldb19.git
```
- If you're running macOS 10.14 you also have to install C/C++ headers by typing the command below and going through the installation screen:
```bash
    $ open /Library/Developer/CommandLineTools/Packages/macOS_SDK_headers_for_macOS_10.14.pkg
```
- Mono Runtime and Compiler: Install the package provided by Mono in https://www.mono-project.com/download/stable/
- All other prerequisites will be installed using a build script.

### Build & tests

- Restart the terminal window after all the dependencies are installed. Open it in the root folder of the repository.
- Build all the algorithms and Testing Framework using a script in the root folder (takes up to 10-12 minutes depending which prerequisites are already installed in the system):
```bash
    $ sh install_mac.sh
```
- Run the benchmark:
```bash
    $ cd TestingFramework/bin/Debug/
    $ mono TestingFramework.exe
```

### Customize datasets and algorithms

The process is identical to Linux.
-->

