﻿# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for license information.

from enum import IntEnum


class CdmObjectType(IntEnum):
    ERROR = 1
    IMPORT = 2
    ARGUMENT_DEF = 3
    PARAMETER_DEF = 4
    TRAIT_DEF = 5
    TRAIT_REF = 6
    PURPOSE_DEF = 7
    PURPOSE_REF = 8
    DATA_TYPE_DEF = 9
    DATA_TYPE_REF = 10
    ATTRIBUTE_REF = 11
    TYPE_ATTRIBUTE_DEF = 12
    ENTITY_ATTRIBUTE_DEF = 13
    ATTRIBUTE_GROUP_DEF = 14
    ATTRIBUTE_GROUP_REF = 15
    CONSTANT_ENTITY_DEF = 16
    ENTITY_DEF = 17
    ENTITY_REF = 18
    DOCUMENT_DEF = 19
    MANIFEST_DEF = 20
    FOLDER_DEF = 21
    ATTRIBUTE_CONTEXT_DEF = 22
    ATTRIBUTE_CONTEXT_REF = 23
    MANIFEST_DECLARATION_DEF = 24
    REFERENCED_ENTITY_DECLARATION_DEF = 25
    DATA_PARTITION_DEF = 26
    DATA_PARTITION_PATTERN_DEF = 27
    LOCAL_ENTITY_DECLARATION_DEF = 28
    ATTRIBUTE_RESOLUTION_GUIDANCE_DEF = 29
    E2E_RELATIONSHIP_DEF = 30
