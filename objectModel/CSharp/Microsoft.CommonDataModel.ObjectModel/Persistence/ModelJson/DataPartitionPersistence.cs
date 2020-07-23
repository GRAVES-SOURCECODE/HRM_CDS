﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.CommonDataModel.ObjectModel.Persistence.ModelJson
{
    using Microsoft.CommonDataModel.ObjectModel.Cdm;
    using Microsoft.CommonDataModel.ObjectModel.Enums;
    using Microsoft.CommonDataModel.ObjectModel.Persistence.ModelJson.types;
    using Microsoft.CommonDataModel.ObjectModel.Utilities;
    using Microsoft.CommonDataModel.ObjectModel.Utilities.Logging;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The data partition persistence.
    /// </summary>
    class DataPartitionPersistence
    {
        public static async Task<CdmDataPartitionDefinition> FromData(CdmCorpusContext ctx, Partition obj, List<CdmTraitDefinition> extensionTraitDefList, List<CdmTraitDefinition> localExtensionTraitDefList, CdmFolderDefinition documentFolder)
        {
            var partition = ctx.Corpus.MakeObject<CdmDataPartitionDefinition>(CdmObjectType.DataPartitionDef, obj.Name);

            partition.Description = obj.Description;
            partition.Location = ctx.Corpus.Storage.CreateRelativeCorpusPath(
                ctx.Corpus.Storage.AdapterPathToCorpusPath(obj.Location),
                documentFolder);
            partition.RefreshTime = obj.RefreshTime;
            partition.LastFileModifiedTime = obj.LastFileModifiedTime;
            partition.LastFileStatusCheckTime = obj.LastFileStatusCheckTime;


            if (string.IsNullOrEmpty(partition.Location))
            {
                Logger.Warning(nameof(DataPartitionPersistence), ctx as ResolveContext, $"Couldn't find data partition's location for partition {partition.Name}.", nameof(FromData));
            }

            if (obj.IsHidden == true)
            {
                var isHiddenTrait = ctx.Corpus.MakeRef<CdmTraitReference>(CdmObjectType.TraitRef, "is.hidden", true);
                partition.ExhibitsTraits.Add(isHiddenTrait);
            }

            await Utils.ProcessAnnotationsFromData(ctx, obj, partition.ExhibitsTraits);

            if (obj.FileFormatSettings != null)
            {
                var csvFormatTrait = Utils.CreateCsvTrait(obj.FileFormatSettings, ctx);

                if (csvFormatTrait == null) {
                    Logger.Error(nameof(DataPartitionPersistence), ctx as ResolveContext, "There was a problem while processing csv format settings inside data partition.");

                    return null;
                }

                partition.ExhibitsTraits.Add(csvFormatTrait);
            }
            ExtensionHelper.ProcessExtensionFromJson(ctx, obj, partition.ExhibitsTraits, extensionTraitDefList, localExtensionTraitDefList);

            return partition;
        }

        public static async Task<Partition> ToData(CdmDataPartitionDefinition instance, ResolveOptions resOpt, CopyOptions options)
        {
            var result = new Partition
            {
                Name = instance.Name,
                Description = instance.Description,
                Location = instance.Ctx.Corpus.Storage.CorpusPathToAdapterPath(
                    instance.Ctx.Corpus.Storage.CreateAbsoluteCorpusPath(
                        instance.Location, instance.InDocument)),
                RefreshTime = instance.RefreshTime,
                FileFormatSettings = null,
                LastFileModifiedTime = instance.LastFileModifiedTime,
                LastFileStatusCheckTime = instance.LastFileStatusCheckTime
            };

            if (string.IsNullOrEmpty(result.Location))
            {
                Logger.Warning(nameof(DataPartitionPersistence), instance.Ctx, $"Couldn't find data partition's location for partition {result.Name}.", nameof(ToData));
            }

            await Utils.ProcessAnnotationsToData(instance.Ctx, result, instance.ExhibitsTraits);

            var t2pm = new TraitToPropertyMap(instance);

            var isHiddenTrait = t2pm.FetchTraitReference("is.hidden");
            if (isHiddenTrait != null)
            {
                result.IsHidden = true;
            }

            var csvTrait = t2pm.FetchTraitReference("is.partition.format.CSV");
            if (csvTrait != null)
            {
                var csvFormatSettings = Utils.CreateCsvFormatSettings(csvTrait);

                if (csvFormatSettings != null)
                {
                    result.FileFormatSettings = csvFormatSettings;
                    result.FileFormatSettings.Type = "CsvFormatSettings";
                }
                else
                {
                    Logger.Error(nameof(DataPartitionPersistence), instance.Ctx,
                        "There was a problem while processing csv format trait inside data partition.");

                    return null;
                }
            }

            return result;
        }
    }
}
