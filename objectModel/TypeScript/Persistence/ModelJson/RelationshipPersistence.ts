import { ModelJson } from '..';
import { CdmCorpusContext, CdmE2ERelationship, cdmObjectType, copyOptions, resolveOptions } from '../../internal';
import { Logger } from '../../Utilities/Logging/Logger';
import { SingleKeyRelationship } from './types';

export class RelationshipPersistence {
    public static async fromData(ctx: CdmCorpusContext, obj: SingleKeyRelationship, entitySchemaByName: Map<string, string>)
        : Promise<CdmE2ERelationship> {
        if (obj.$type !== 'SingleKeyRelationship') {
            // We don't have any other type of a relationship yet!
            return;
        }

        if (!entitySchemaByName.has(obj.fromAttribute.entityName)) {
            Logger.warning(
                RelationshipPersistence.name,
                ctx,
                `Relationship's source entity '${obj.fromAttribute.entityName}' is not defined.`
            );

            return;
        }

        if (!entitySchemaByName.has(obj.toAttribute.entityName)) {
            Logger.warning(
                RelationshipPersistence.name,
                ctx,
                `Relationship's target entity '${obj.toAttribute.entityName}' is not defined.`
            );

            return;
        }

        const relationship: CdmE2ERelationship = ctx.corpus.MakeObject<CdmE2ERelationship>(cdmObjectType.e2eRelationshipDef, obj.name);

        await ModelJson.utils.processAnnotationsFromData(ctx, obj, relationship.exhibitsTraits);

        relationship.name = obj.name;
        relationship.explanation = obj.description;

        relationship.fromEntity = entitySchemaByName.get(obj.fromAttribute.entityName);
        relationship.toEntity = entitySchemaByName.get(obj.toAttribute.entityName);
        relationship.fromEntityAttribute = obj.fromAttribute.attributeName;
        relationship.toEntityAttribute = obj.toAttribute.attributeName;

        return relationship;
    }

    public static async toData(instance: CdmE2ERelationship, resOpt: resolveOptions, options: copyOptions, ctx: CdmCorpusContext)
        : Promise<SingleKeyRelationship> {
        const result: SingleKeyRelationship = <SingleKeyRelationship>{};
        result.$type = 'SingleKeyRelationship';
        result.fromAttribute = {
            entityName: RelationshipPersistence.getEntityName(instance.fromEntity),
            attributeName: instance.fromEntityAttribute
        };

        result.toAttribute = {
            entityName: RelationshipPersistence.getEntityName(instance.toEntity),
            attributeName: instance.toEntityAttribute
        };

        result.description = instance.explanation;
        result.name = instance.name;
        await ModelJson.utils.processAnnotationsToData(instance.ctx, result, instance.exhibitsTraits);

        return result;
    }

    private static getEntityName(corpusPath: string): string {
        const lastSlashIndex: number = corpusPath.lastIndexOf('/');

        if (lastSlashIndex !== -1) {
            return corpusPath.slice(lastSlashIndex + 1);
        }

        return corpusPath;
    }
}
