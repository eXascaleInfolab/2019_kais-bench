brew update
brew install --force-bottle llvm
brew install cmake openblas lapack armadillo boost
chmod u+x mac_install_mlpack.sh
./mac_install_mlpack.sh
brew install R gnuplot
brew install python2
pip2 install numpy scipy pandas sklearn tensorflow
python mac_build.py