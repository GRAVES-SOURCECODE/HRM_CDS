// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

package com.microsoft.commondatamodel.objectmodel.persistence.cdmfolder;

import com.fasterxml.jackson.databind.JsonNode;
import com.microsoft.commondatamodel.objectmodel.cdm.CdmCorpusContext;
import com.microsoft.commondatamodel.objectmodel.cdm.CdmDataTypeReference;
import com.microsoft.commondatamodel.objectmodel.cdm.CdmTraitReference;
import com.microsoft.commondatamodel.objectmodel.enums.CdmObjectType;
import com.microsoft.commondatamodel.objectmodel.persistence.cdmfolder.types.DataType;
import com.microsoft.commondatamodel.objectmodel.utilities.CopyOptions;
import com.microsoft.commondatamodel.objectmodel.utilities.JMapper;
import com.microsoft.commondatamodel.objectmodel.utilities.ResolveOptions;
import com.microsoft.commondatamodel.objectmodel.utilities.logger.Logger;

import java.io.IOException;
import java.util.List;

public class DataTypeReferencePersistence {
  public static CdmDataTypeReference fromData(final CdmCorpusContext ctx, final JsonNode obj) {
        if (obj == null) {
            return null;
        }

        boolean simpleReference = true;
        Object dataType = null;
        List<CdmTraitReference> appliedTraits = null;

        if (obj.isValueNode())
           dataType = obj;
        else {
            simpleReference = false;
            if (obj.get("dataTypeReference").isValueNode())
                dataType = obj.get("dataTypeReference").asText();
            else
                try {
                    dataType = DataTypePersistence.fromData(ctx, JMapper.MAP.treeToValue(obj.get("dataTypeReference"), DataType.class));
                } catch (final IOException ex) {
                    Logger.error(
                        DataTypeReferencePersistence.class.getSimpleName(),
                        ctx,
                        Logger.format("There was an error while trying to convert from JSON to DataTypeRef. Reason: '{0}'", ex.getLocalizedMessage()),
                        "fromData"
                    );
                }
        }

        final CdmDataTypeReference dataTypeReference = ctx.getCorpus().makeRef(CdmObjectType.DataTypeRef, dataType, simpleReference);

        if (!obj.isValueNode())
            appliedTraits = Utils.createTraitReferenceList(ctx, obj.get("appliedTraits"));

        Utils.addListToCdmCollection(dataTypeReference.getAppliedTraits(), appliedTraits);

        return dataTypeReference;
    }

  public static Object toData(final CdmDataTypeReference instance, final ResolveOptions resOpt, final CopyOptions options) {
        return CdmObjectRefPersistence.toData(instance, resOpt, options);
    }
}
