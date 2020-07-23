# ----------------------------------------------------------------------
# Copyright (c) Microsoft Corporation.
# All rights reserved.
# ----------------------------------------------------------------------

# TODO: Consider just inheriting from Python's set type -MPL


class SymbolSet:
    def __init__(self):
        self._symbol_set_collection = set()

    @property
    def size(self):
        return len(self._symbol_set_collection)

    def add(self, new_symbol):
        self._symbol_set_collection.add(new_symbol)

    def merge(self, sym_set):
        if sym_set is not None:
            self._symbol_set_collection = self._symbol_set_collection.union(sym_set)

    def copy(self):
        return self._symbol_set_collection.copy()

    def __iter__(self):
        return iter(self._symbol_set_collection)
