import {
  Button,
  FormControl,
  FormControlLabel,
  FormLabel,
  Radio,
  RadioGroup,
  Rating,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { useState } from "react";
import { t } from "../../../../shared/i18n";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import type { ReviewTarget } from "../api/reviewTypes";
import { useCreateReview } from "../hooks/useCreateReview";
import { reviewTargetLabel } from "../model/reviewDisplay";
import { parseTargetKey, targetKey } from "../model/reviewTargets";

type CreateReviewFormProps = {
  dealId: number;
  targets: ReviewTarget[];
  disabled: boolean;
};

const COMMENT_MAX = 2000;

export function CreateReviewForm({ dealId, targets, disabled }: CreateReviewFormProps) {
  const create = useCreateReview(dealId);
  const [rating, setRating] = useState<number | null>(null);
  const [comment, setComment] = useState("");
  const [selectedKey, setSelectedKey] = useState(targets[0] ? targetKey(targets[0]) : "");
  const [clientError, setClientError] = useState<string | null>(null);

  const selected =
    targets.length === 1 ? targets[0] : parseTargetKey(selectedKey) ?? targets[0];

  if (create.isSuccess) {
    return (
      <Typography variant="body2" color="success.main">
        {t("review.form.success")}
      </Typography>
    );
  }

  return (
    <Stack
      component="form"
      spacing={2}
      onSubmit={(event) => {
        event.preventDefault();
        if (disabled || !selected) {
          return;
        }
        if (rating == null || rating < 1 || rating > 5) {
          setClientError(t("review.form.ratingRequired"));
          return;
        }
        if (comment.length > COMMENT_MAX) {
          setClientError(t("review.form.commentTooLong"));
          return;
        }
        setClientError(null);
        create.mutate({
          rating,
          comment: comment.trim() ? comment.trim() : null,
          businessId: selected.kind === "Business" ? selected.id : null,
          providerId: selected.kind === "Provider" ? selected.id : null,
        });
      }}
    >
      <Typography variant="subtitle1">{t("review.form.title")}</Typography>
      {targets.length > 1 ? (
        <FormControl>
          <FormLabel>{t("review.form.target")}</FormLabel>
          <RadioGroup
            value={selected ? targetKey(selected) : ""}
            onChange={(event) => {
              setSelectedKey(event.target.value);
            }}
          >
            {targets.map((target) => (
              <FormControlLabel
                key={targetKey(target)}
                value={targetKey(target)}
                control={<Radio />}
                label={reviewTargetLabel(target)}
              />
            ))}
          </RadioGroup>
        </FormControl>
      ) : selected ? (
        <Typography variant="body2">{reviewTargetLabel(selected)}</Typography>
      ) : null}

      <Stack spacing={0.5}>
        <Typography variant="body2">{t("review.form.rating")}</Typography>
        <Typography variant="caption" color="text.secondary">
          {t("review.form.ratingHint")}
        </Typography>
        <Rating
          name="review-rating"
          max={5}
          value={rating}
          onChange={(_, value) => {
            setRating(value);
          }}
          disabled={disabled || create.isPending}
        />
      </Stack>

      <TextField
        label={t("review.form.comment")}
        value={comment}
        onChange={(event) => {
          setComment(event.target.value);
        }}
        multiline
        minRows={3}
        inputProps={{ maxLength: COMMENT_MAX }}
        disabled={disabled || create.isPending}
      />

      {clientError ? <Typography color="error">{clientError}</Typography> : null}
      {create.isError ? <ErrorAlert error={create.error} /> : null}

      <Button
        type="submit"
        variant="contained"
        disabled={disabled || create.isPending || !selected}
        sx={{ alignSelf: "flex-start" }}
      >
        {create.isPending ? t("review.form.submitting") : t("review.form.submit")}
      </Button>
    </Stack>
  );
}
