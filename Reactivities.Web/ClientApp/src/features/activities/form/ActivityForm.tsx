import * as React from 'react';
import { Form, Formik } from 'formik';
import { observer } from 'mobx-react-lite';
import { useEffect, useState } from 'react';
import { Link, useHistory, useParams } from 'react-router-dom';
import { Button, Header, Segment } from 'semantic-ui-react';
import * as Yup from 'yup';
import DateInput from '../../../app/common/form/DateInput';
import SelectInput from '../../../app/common/form/SelectInput';
import TextArea from '../../../app/common/form/TextArea';
import TextInput from '../../../app/common/form/TextInput';
import { categoryOptions } from '../../../app/common/options/categoryOptions';
import LoadingComponent from '../../../app/layout/LoadingComponent';
import { Activity, ActivityFormValues } from '../../../app/models/activity';
import { useStore } from '../../../app/stores/store';
import { v4 as uuid } from 'uuid';

function ActivityForm() {

    const { activityStore } = useStore();
    const { loading, loadActivity, loadingInitial, createActivity, updateActivity } = activityStore;
    const { id } = useParams<{ id: string }>();

    const history = useHistory();

    const [activity, setActivity] = useState<ActivityFormValues>(new ActivityFormValues);

    const validationSchema = Yup.object({
        title: Yup.string().required('The activity title is required'),
        description: Yup.string().required('The activity description is required'),
        category: Yup.string().required('The activity category is required'),
        date: Yup.string().required('The activity date is required').nullable(),
        city: Yup.string().required('The activity city is required'),
        venue: Yup.string().required('The activity venue is required'),
    })

    useEffect(() => {
        if (id) loadActivity(id).then(activity => setActivity(new ActivityFormValues(activity)));
    }, [id, loadActivity]);

    function handleFormSubmit(activity: ActivityFormValues) {
        if (!activity.id) {
            let newActivity = {
                ...activity,
                id: uuid()
            };
            createActivity(newActivity).then(() => history.push(`/activities/${newActivity.id}`));
        } else {
            updateActivity(activity).then(() => history.push(`/activities/${activity.id}`));
        }
    }

    if (loadingInitial) return <LoadingComponent content="Loading..." />

    return (
        <Segment clearing>
            <Header content='Activity Details' sub color='teal' />
            <Formik
                enableReinitialize
                initialValues={activity}
                onSubmit={val => handleFormSubmit(val)}
                validationSchema={validationSchema}
            >
                {({ handleSubmit, isValid, isSubmitting, dirty }) => (

                    <Form className='ui form' onSubmit={handleSubmit} autoComplete='off'>
                        <TextInput name='title' placeholder='Title' />
                        <TextArea rows={3} placeholder='Description' name='description' />
                        <SelectInput options={categoryOptions} placeholder='Category' name='category' />
                        <DateInput
                            placeholderText='Date'
                            name='date' showTimeSelect
                            timeCaption='time'
                            dateFormat='MMMM d, yyyy h:mm aa'
                        />
                        <Header content='Location Details' sub color='teal' />
                        <TextInput placeholder='City' name='city' />
                        <TextInput placeholder='Venue' name='venue' />
                        <Button
                            disabled={!isValid || isSubmitting || !dirty}
                            loading={isSubmitting} floated='right' positive type='submit' content='Submit' />
                        <Button as={Link} to='/activities' floated='right' type='button' content='Cancel' />
                    </Form>
                )}
            </Formik>
        </Segment>
    );
}

export default observer(ActivityForm);